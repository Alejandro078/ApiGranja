using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Services.Dtos;
using Services.Services;
using Xunit;

namespace Granja.Tests;

public class ServiciosTests
{
    private sealed class RelojPrueba : IClock
    {
        public DateOnly Today { get; init; } = new(2026, 12, 3);
    }

    private static AppDbContext CrearContexto(SqliteConnection conexion)
    {
        var opciones = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(conexion).Options;
        var contexto = new AppDbContext(opciones);
        contexto.Database.EnsureCreated();
        return contexto;
    }

    private static async Task PrepararGranjaAsync(AppDbContext db, int numeroVacas = 2)
    {
        db.Granjas.Add(new Domain.Granja(numeroVacas, 15m, 10m, 1m, 30, 15, 28000m, 1000m));
        for (var i = 0; i < numeroVacas; i++) db.Cows.Add(new Cow { Race = "Holstein" });
        await db.SaveChangesAsync();
    }

    private static IMapper CrearMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<LoteProducto, LoteInventarioResponse>()
                .ForMember(d => d.TipoProducto, o => o.MapFrom(s => s.Tipo));
        }, NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    [Fact]
    public async Task ProduccionDiariaEsNumeroVacasPorLitrosYNoSeDuplica()
    {
        await using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();
        await using var db = CrearContexto(conexion);
        await PrepararGranjaAsync(db, 2);
        var service = new ProduccionService(new GranjaRepository(db), new CowRepository(db),
            new ProduccionLecheRepository(db), new LoteRepository(db), new UnitOfWork(db), new RelojPrueba());

        var resultado = await service.RegistrarProduccionDiariaAsync(new ProduccionRequest { Fecha = new DateOnly(2026, 12, 1) });
        Assert.Equal(30m, resultado.LitrosTotales);
        Assert.Equal(2, await db.Producciones.CountAsync());
        await Assert.ThrowsAsync<ConflictoOperacionException>(() =>
            service.RegistrarProduccionDiariaAsync(new ProduccionRequest { Fecha = new DateOnly(2026, 12, 1) }));
    }

    [Fact]
    public async Task NoPermiteTransformarMasLecheDeLaProducida()
    {
        await using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();
        await using var db = CrearContexto(conexion);
        await PrepararGranjaAsync(db, 2);
        var service = new ProduccionService(new GranjaRepository(db), new CowRepository(db),
            new ProduccionLecheRepository(db), new LoteRepository(db), new UnitOfWork(db), new RelojPrueba());
        await service.RegistrarProduccionDiariaAsync(new ProduccionRequest { Fecha = new DateOnly(2026, 12, 1) });

        await Assert.ThrowsAsync<LecheInsuficienteException>(() => service.TransformarLecheAsync(
            new TransformacionRequest { Fecha = new DateOnly(2026, 12, 1), TipoProducto = TipoProducto.Queso, Litros = 31m }));
        Assert.Empty(await db.Lotes.ToListAsync());
        Assert.Equal(0m, (await db.Producciones.Select(p => p.LitrosTransformados).ToListAsync()).Sum());
    }

    [Fact]
    public async Task VentaUtilizaFIFO_YPuedeTomarDeVariosLotes()
    {
        await using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();
        await using var db = CrearContexto(conexion);
        await PrepararGranjaAsync(db);
        var primero = new LoteProducto(TipoProducto.Queso, new DateOnly(2026, 12, 1), 10m, 30);
        var segundo = new LoteProducto(TipoProducto.Queso, new DateOnly(2026, 12, 2), 15m, 30);
        db.Lotes.AddRange(primero, segundo);
        await db.SaveChangesAsync();
        var service = new VentaService(new LoteRepository(db), new VentaRepository(db), new GranjaRepository(db),
            new UnitOfWork(db), new RelojPrueba());

        var venta = await service.RegistrarVentaAsync(new VentaRequest { TipoProducto = TipoProducto.Queso, Cantidad = 12m, Fecha = new DateOnly(2026, 12, 3) });

        Assert.Equal(2, venta.Detalle.Count);
        Assert.Equal(10m, venta.Detalle[0].Cantidad);
        Assert.Equal(2m, venta.Detalle[1].Cantidad);
        Assert.Equal(0m, (await db.Lotes.FindAsync(primero.Id))!.CantidadDisponible);
        Assert.Equal(13m, (await db.Lotes.FindAsync(segundo.Id))!.CantidadDisponible);
    }

    [Fact]
    public async Task NoVendeLotesVencidos()
    {
        await using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();
        await using var db = CrearContexto(conexion);
        await PrepararGranjaAsync(db);
        var lote = new LoteProducto(TipoProducto.Yogurt, new DateOnly(2026, 11, 1), 5m, 15);
        db.Lotes.Add(lote);
        await db.SaveChangesAsync();
        var service = new VentaService(new LoteRepository(db), new VentaRepository(db), new GranjaRepository(db),
            new UnitOfWork(db), new RelojPrueba { Today = new DateOnly(2026, 12, 3) });

        await Assert.ThrowsAsync<InventarioInsuficienteException>(() => service.RegistrarVentaAsync(
            new VentaRequest { TipoProducto = TipoProducto.Yogurt, Cantidad = 1m, Fecha = new DateOnly(2026, 12, 3) }));
        Assert.Equal(5m, (await db.Lotes.FindAsync(lote.Id))!.CantidadDisponible);
        Assert.Empty(await db.Ventas.ToListAsync());
    }

    [Fact]
    public async Task VentaInsuficienteNoModificaInventario()
    {
        await using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();
        await using var db = CrearContexto(conexion);
        await PrepararGranjaAsync(db);
        var lote1 = new LoteProducto(TipoProducto.Queso, new DateOnly(2026, 12, 1), 5m, 30);
        var lote2 = new LoteProducto(TipoProducto.Queso, new DateOnly(2026, 12, 2), 5m, 30);
        db.Lotes.AddRange(lote1, lote2);
        await db.SaveChangesAsync();
        var service = new VentaService(new LoteRepository(db), new VentaRepository(db), new GranjaRepository(db),
            new UnitOfWork(db), new RelojPrueba());

        await Assert.ThrowsAsync<InventarioInsuficienteException>(() => service.RegistrarVentaAsync(
            new VentaRequest { TipoProducto = TipoProducto.Queso, Cantidad = 11m, Fecha = new DateOnly(2026, 12, 3) }));
        Assert.Equal(10m, (await db.Lotes.Select(l => l.CantidadDisponible).ToListAsync()).Sum());
        Assert.Empty(await db.Ventas.ToListAsync());
    }

    [Fact]
    public async Task ProcesarVencimientosPasaExistenciasADanadoYReporteLasMuestra()
    {
        await using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();
        await using var db = CrearContexto(conexion);
        await PrepararGranjaAsync(db);
        var lote = new LoteProducto(TipoProducto.Yogurt, new DateOnly(2026, 11, 1), 5m, 15);
        db.Lotes.Add(lote);
        await db.SaveChangesAsync();
        var inventory = new InventarioService(new LoteRepository(db), new UnitOfWork(db), new RelojPrueba(), CrearMapper());

        var procesado = await inventory.ProcesarVencimientosAsync(new DateOnly(2026, 12, 3));
        var guardado = await db.Lotes.FindAsync(lote.Id);
        var reportes = new ReporteService(new VentaRepository(db), new ProduccionLecheRepository(db),
            new LoteRepository(db), new CowRepository(db));
        var danados = await reportes.ObtenerProductoDanadoAsync(null, null);

        Assert.Equal(5m, procesado.CantidadDanada);
        Assert.Equal(0m, guardado!.CantidadDisponible);
        Assert.Equal(5m, guardado.CantidadDanada);
        Assert.Single(danados);
        Assert.Equal(5m, danados[0].CantidadDanada);
    }
}
