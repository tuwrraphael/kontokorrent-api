using System;

namespace Kontokorrent.ApiModels.v2
{
    public class Bezahlung
    {
        public string Id { get; set; }
        public DateTimeOffset Zeitpunkt { get; set; }
        public string BezahlendePersonId { get; set; }
        public string[] EmpfaengerIds { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
    }
}
