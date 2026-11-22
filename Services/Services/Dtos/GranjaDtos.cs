namespace Services.Dtos;

public class GranjaRequest
{
    public int NumeroVacas { get; set; }
    public decimal LitrosPorVacaDia { get; set; }
    public decimal RendimientoQueso { get; set; }
    public decimal RendimientoYogurt { get; set; }
    public int VidaUtilQueso { get; set; }
    public int VidaUtilYogurt { get; set; }
    public decimal PrecioQueso { get; set; }
    public decimal PrecioYogurt { get; set; }
}

public class GranjaResponse
{
    public int Id { get; set; }
    public int NumeroVacas { get; set; }
    public decimal LitrosPorVacaDia { get; set; }
    public decimal ProduccionDiariaTotal { get; set; }
    public decimal RendimientoQueso { get; set; }
    public decimal RendimientoYogurt { get; set; }
    public int VidaUtilQueso { get; set; }
    public int VidaUtilYogurt { get; set; }
    public decimal PrecioQueso { get; set; }
    public decimal PrecioYogurt { get; set; }
}
