using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ICowRepository, CowRepository>();
        services.AddScoped<IGranjaRepository, GranjaRepository>();
        services.AddScoped<IProduccionLecheRepository, ProduccionLecheRepository>();
        services.AddScoped<ILoteRepository, LoteRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
