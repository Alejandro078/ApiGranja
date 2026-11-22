using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Services.Services;

namespace Services;

public static class Dependencyinjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IGranjaService, GranjaService>();
        services.AddScoped<IProduccionService, ProduccionService>();
        services.AddScoped<IInventarioService, InventarioService>();
        services.AddScoped<IVentaService, VentaService>();
        services.AddScoped<IReporteService, ReporteService>();
        return services;
    }
}
