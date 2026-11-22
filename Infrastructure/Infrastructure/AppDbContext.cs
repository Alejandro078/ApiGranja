using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cow> Cows => Set<Cow>();
    public DbSet<ProduccionLeche> Producciones => Set<ProduccionLeche>();
    public DbSet<Granja> Granjas => Set<Granja>();
    public DbSet<LoteProducto> Lotes => Set<LoteProducto>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<VentaDetalle> VentasDetalles => Set<VentaDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cow>().Property(c => c.Race).IsRequired();
        modelBuilder.Entity<ProduccionLeche>()
            .HasOne<Cow>().WithMany(c => c.Producciones)
            .HasForeignKey(p => p.CowId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ProduccionLeche>()
            .HasIndex(p => new { p.CowId, p.Fecha }).IsUnique();

        modelBuilder.Entity<Venta>()
            .HasMany(v => v.Detalles).WithOne(d => d.Venta)
            .HasForeignKey(d => d.VentaId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<VentaDetalle>()
            .HasOne(d => d.Lote).WithMany()
            .HasForeignKey(d => d.LoteId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProduccionLeche>().Property(p => p.LitrosProducidos).HasPrecision(18, 3);
        modelBuilder.Entity<ProduccionLeche>().Property(p => p.LitrosTransformados).HasPrecision(18, 3);
        modelBuilder.Entity<LoteProducto>().Property(l => l.CantidadInicial).HasPrecision(18, 3);
        modelBuilder.Entity<LoteProducto>().Property(l => l.CantidadDisponible).HasPrecision(18, 3);
        modelBuilder.Entity<LoteProducto>().Property(l => l.CantidadDanada).HasPrecision(18, 3);
        modelBuilder.Entity<Venta>().Property(v => v.Cantidad).HasPrecision(18, 3);
        modelBuilder.Entity<Venta>().Property(v => v.PrecioUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<Venta>().Property(v => v.Total).HasPrecision(18, 2);
        modelBuilder.Entity<VentaDetalle>().Property(v => v.Cantidad).HasPrecision(18, 3);
    }
}
