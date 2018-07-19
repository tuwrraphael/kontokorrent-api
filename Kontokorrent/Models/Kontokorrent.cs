namespace Kontokorrent.Models
{
    public class KontokorrentStatus
    {
        public PersonenStatus[] PersonenStatus { get; set; }
        public Bezahlung[] LetzteBezahlungen { get; set; }
    }
}
