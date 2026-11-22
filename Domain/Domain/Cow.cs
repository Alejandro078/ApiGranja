namespace Domain
{
    public class Cow
    {
        public int Id { get; set; }

        public string Race { get; set; } = string.Empty;

        // Una vaca puede registrar varias producciones, una por cada fecha.
        public ICollection<ProduccionLeche> Producciones { get; set; }
            = new List<ProduccionLeche>();
    }
}
