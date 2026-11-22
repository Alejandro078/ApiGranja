using ApiSampleFinal.Automapper;
using ApiSampleFinal.Middleware;
using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Services;
using Services.Interfaces;
using System.Text.Json.Serialization;

namespace ApiSampleFinal;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddServices();
        builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddAuthorization();
        builder.Services.AddRepositories(configuration);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var mappingConfiguration = new MapperConfiguration(m => m.AddProfile(new MappingProfile()), NullLoggerFactory.Instance);
        builder.Services.AddSingleton(mappingConfiguration.CreateMapper());

        builder.Services.AddCors(options => options.AddPolicy("CORS_Policy", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
            // La configuración inicial usa los valores sugeridos del enunciado.
            await scope.ServiceProvider.GetRequiredService<IGranjaService>().ObtenerConfiguracionAsync();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("CORS_Policy");
        if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}
