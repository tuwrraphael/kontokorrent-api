﻿using System;

namespace Kontokorrent.Models
{
    public class Bezahlung
    {
        public string Id { get; set; }
        public DateTimeOffset Zeitpunkt { get; set; }
        public Person BezahlendePerson { get; set; }
        public Person[] Empfaenger { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
    }
}
