using System;

namespace Kontokorrent.Models
{
    public class GeaenderteBezahlung
    {
        public string[] EmpfaengerIds { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        public DateTimeOffset Zeitpunkt { get; set; }
        public string BezahlendePersonId { get; set; }
    }
}
