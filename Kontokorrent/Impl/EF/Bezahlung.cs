using System;
using System.Collections.Generic;

namespace Kontokorrent.Impl.EF
{
    public class Bezahlung
    {
        public string Id { get; set; }
        public string KontokorrentId { get; set; }
        public int LaufendeNummer { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
        public string BezahlendePersonId { get; set; }
        public Person BezahlendePerson { get; set; }
        public List<EmfpaengerInBezahlung> Emfpaenger { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        public DateTime Zeitpunkt { get; set; }
        public string BearbeiteteBezahlungId { get; set; }
        public Bezahlung BearbeiteteBezahlung { get; set; }
        public List<Bezahlung> BearbeitendeBezahlungen { get; set; }
        public string GeloeschteBezahlungId { get; set; }
        public Bezahlung GeloeschteBezahlung { get; set; }
        public List<Bezahlung> LoeschendeBezahlungen { get; set; }
    }
}
