namespace Domain;

public class ProduccionLeche
{
    public int Id { get; private set; }
    public int CowId { get; private set; }
    public DateOnly Fecha { get; private set; }
    public decimal LitrosProducidos { get; private set; }
    public decimal LitrosTransformados { get; private set; }
    public decimal LitrosNoAprovechados => LitrosProducidos - LitrosTransformados;

    public ProduccionLeche(int cowId, DateOnly fecha, decimal litros)
    {
        if (cowId <= 0) throw new ArgumentException("Vaca inválida.");
        if (litros <= 0) throw new ArgumentException("Los litros deben ser mayores a cero.");
        CowId = cowId;
        Fecha = fecha;
        LitrosProducidos = litros;
    }

    private ProduccionLeche() { }

    public void RegistrarTransformacion(decimal litros)
    {
        if (litros <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        if (LitrosTransformados + litros > LitrosProducidos)
            throw new InvalidOperationException("No hay suficiente leche disponible.");
        LitrosTransformados += litros;
    }
}
