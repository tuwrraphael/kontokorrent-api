namespace Kontokorrent.Models
{
    public class AusgleichRequest
    {
        public GeforderteZahlung[] BevorzugteZahlungen { get; set; }
        public GeforderteZahlung[] MussZahlungen { get; set; }
    }
}
