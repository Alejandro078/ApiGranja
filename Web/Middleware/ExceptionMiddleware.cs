using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiSampleFinal.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var status = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                InventarioInsuficienteException => StatusCodes.Status409Conflict,
                LecheInsuficienteException => StatusCodes.Status409Conflict,
                ConflictoOperacionException => StatusCodes.Status409Conflict,
                InvalidOperationException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Error no controlado al procesar la solicitud.");

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            var problem = new ProblemDetails
            {
                Status = status,
                Title = status switch
                {
                    StatusCodes.Status400BadRequest => "Solicitud inválida",
                    StatusCodes.Status404NotFound => "Recurso no encontrado",
                    StatusCodes.Status409Conflict => "Conflicto con el estado actual",
                    _ => "Error interno del servidor"
                },
                Detail = status == StatusCodes.Status500InternalServerError ? "Ocurrió un error interno." : exception.Message,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
