using System;

namespace Kontokorrent.Models
{
    public class NeueBezahlung
    {
        public string BezahlendePersonId { get; set; }
        public string[] EmpfaengerIds { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        public DateTimeOffset Zeitpunkt { get; set; }
        public string Id { get; set; }
    }
}
