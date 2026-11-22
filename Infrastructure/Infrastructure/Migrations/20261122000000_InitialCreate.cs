using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261122000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Cows",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Race = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Cows", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Granjas",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                NumeroVacas = table.Column<int>(type: "INTEGER", nullable: false),
                LitrosPorVacaDia = table.Column<decimal>(type: "TEXT", nullable: false),
                RendimientoQueso = table.Column<decimal>(type: "TEXT", nullable: false),
                RendimientoYogurt = table.Column<decimal>(type: "TEXT", nullable: false),
                VidaUtilQueso = table.Column<int>(type: "INTEGER", nullable: false),
                VidaUtilYogurt = table.Column<int>(type: "INTEGER", nullable: false),
                PrecioQueso = table.Column<decimal>(type: "TEXT", nullable: false),
                PrecioYogurt = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Granjas", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Lotes",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                FechaProduccion = table.Column<DateOnly>(type: "TEXT", nullable: false),
                FechaVencimiento = table.Column<DateOnly>(type: "TEXT", nullable: false),
                CantidadInicial = table.Column<decimal>(type: "TEXT", nullable: false),
                CantidadDisponible = table.Column<decimal>(type: "TEXT", nullable: false),
                CantidadDanada = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Lotes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Producciones",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                CowId = table.Column<int>(type: "INTEGER", nullable: false),
                Fecha = table.Column<DateOnly>(type: "TEXT", nullable: false),
                LitrosProducidos = table.Column<decimal>(type: "TEXT", nullable: false),
                LitrosTransformados = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Producciones", x => x.Id);
                table.ForeignKey("FK_Producciones_Cows_CowId", x => x.CowId, "Cows", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Ventas",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Fecha = table.Column<DateOnly>(type: "TEXT", nullable: false),
                TipoProducto = table.Column<int>(type: "INTEGER", nullable: false),
                Cantidad = table.Column<decimal>(type: "TEXT", nullable: false),
                PrecioUnitario = table.Column<decimal>(type: "TEXT", nullable: false),
                Total = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Ventas", x => x.Id));

        migrationBuilder.CreateTable(
            name: "VentasDetalles",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                VentaId = table.Column<int>(type: "INTEGER", nullable: false),
                LoteId = table.Column<int>(type: "INTEGER", nullable: false),
                Cantidad = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VentasDetalles", x => x.Id);
                table.ForeignKey("FK_VentasDetalles_Lotes_LoteId", x => x.LoteId, "Lotes", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_VentasDetalles_Ventas_VentaId", x => x.VentaId, "Ventas", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Producciones_CowId_Fecha", table: "Producciones", columns: new[] { "CowId", "Fecha" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_VentasDetalles_LoteId", table: "VentasDetalles", column: "LoteId");
        migrationBuilder.CreateIndex(name: "IX_VentasDetalles_VentaId", table: "VentasDetalles", column: "VentaId");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.15");

        modelBuilder.Entity<Cow>(entity =>
        {
            entity.ToTable("Cows");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Race).IsRequired();
        });

        modelBuilder.Entity<Granja>(entity =>
        {
            entity.ToTable("Granjas");
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<LoteProducto>(entity =>
        {
            entity.ToTable("Lotes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CantidadInicial).HasPrecision(18, 3);
            entity.Property(x => x.CantidadDisponible).HasPrecision(18, 3);
            entity.Property(x => x.CantidadDanada).HasPrecision(18, 3);
        });

        modelBuilder.Entity<ProduccionLeche>(entity =>
        {
            entity.ToTable("Producciones");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.CowId, x.Fecha }).IsUnique();
            entity.Property(x => x.LitrosProducidos).HasPrecision(18, 3);
            entity.Property(x => x.LitrosTransformados).HasPrecision(18, 3);
            entity.HasOne<Cow>().WithMany(x => x.Producciones).HasForeignKey(x => x.CowId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("Ventas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Cantidad).HasPrecision(18, 3);
            entity.Property(x => x.PrecioUnitario).HasPrecision(18, 2);
            entity.Property(x => x.Total).HasPrecision(18, 2);
            entity.HasMany(x => x.Detalles).WithOne(x => x.Venta).HasForeignKey(x => x.VentaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VentaDetalle>(entity =>
        {
            entity.ToTable("VentasDetalles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Cantidad).HasPrecision(18, 3);
            entity.HasOne(x => x.Lote).WithMany().HasForeignKey(x => x.LoteId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Producciones");
        migrationBuilder.DropTable(name: "VentasDetalles");
        migrationBuilder.DropTable(name: "Granjas");
        migrationBuilder.DropTable(name: "Cows");
        migrationBuilder.DropTable(name: "Lotes");
        migrationBuilder.DropTable(name: "Ventas");
    }
}
