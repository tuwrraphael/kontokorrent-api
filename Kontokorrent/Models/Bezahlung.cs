using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Models
{
    public class Bezahlung
    {
        public string Id { get; set; }
        public DateTime Zeitpunkt { get; set; }
        public Person BezahlendePerson { get; set; }
        public Person[] Empfaenger { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
    }
}
