using System;
using System.Collections.Generic;

namespace Kontokorrent.Impl.EF
{
    public class Bezahlung
    {
        public string Id { get; set; }
        public string KontokorrentId { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
        public string BezahlendePersonId { get; set; }
        public Person BezahlendePerson { get; set; }
        public List<EmfpaengerInBezahlung> Emfpaenger { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        public DateTime Zeitpunkt { get; set; }
        public bool Deleted { get; set; }
    }
}
