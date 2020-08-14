namespace Kontokorrent.ApiModels.v1
{
    public class KontokorrentResponse
    {
        public PersonenStatus[] PersonenStatus { get; set; }
        public Bezahlung[] LetzteBezahlungen { get; set; }
    }
}
