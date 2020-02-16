namespace Kontokorrent.Models
{
    public class Ausgleichszahlung
    {
        public Person BezahlendePerson { get; set; }
        public Person Empfaenger { get; set; }
        public double Wert { get; set; }
    }
}
