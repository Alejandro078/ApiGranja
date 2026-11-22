namespace Domain;

public class Granja
{
    public int Id { get; private set; }
    public int NumeroVacas { get; private set; }
    public decimal LitrosPorVacaDia { get; private set; }
    public decimal RendimientoQueso { get; private set; }
    public decimal RendimientoYogurt { get; private set; }
    public int VidaUtilQueso { get; private set; }
    public int VidaUtilYogurt { get; private set; }
    public decimal PrecioQueso { get; private set; }
    public decimal PrecioYogurt { get; private set; }

    private Granja() { }

    public Granja(int numeroVacas, decimal litrosPorVacaDia, decimal rendimientoQueso,
        decimal rendimientoYogurt, int vidaUtilQueso, int vidaUtilYogurt,
        decimal precioQueso, decimal precioYogurt)
    {
        ActualizarConfiguracion(numeroVacas, litrosPorVacaDia, rendimientoQueso,
            rendimientoYogurt, vidaUtilQueso, vidaUtilYogurt, precioQueso, precioYogurt);
    }

    public decimal CalcularProduccionDiaria() => NumeroVacas * LitrosPorVacaDia;

    public void ActualizarConfiguracion(int numeroVacas, decimal litrosPorVacaDia,
        decimal rendimientoQueso, decimal rendimientoYogurt, int vidaUtilQueso,
        int vidaUtilYogurt, decimal precioQueso, decimal precioYogurt)
    {
        if (numeroVacas <= 0 || litrosPorVacaDia <= 0 || rendimientoQueso <= 0 ||
            rendimientoYogurt <= 0 || vidaUtilQueso <= 0 || vidaUtilYogurt <= 0 ||
            precioQueso < 0 || precioYogurt < 0)
            throw new ArgumentException("Los parámetros de la granja no son válidos.");

        NumeroVacas = numeroVacas;
        LitrosPorVacaDia = litrosPorVacaDia;
        RendimientoQueso = rendimientoQueso;
        RendimientoYogurt = rendimientoYogurt;
        VidaUtilQueso = vidaUtilQueso;
        VidaUtilYogurt = vidaUtilYogurt;
        PrecioQueso = precioQueso;
        PrecioYogurt = precioYogurt;
    }
}
