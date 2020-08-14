using System;
using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v1
{
    public class NeueBezahlungRequest
    {
        [Required]
        public string BezahlendePerson { get; set; }
        [Required]
        public string[] Empfaenger { get; set; }
        [Required]
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        public DateTime? Zeitpunkt { get; set; }
    }

    public class BezahlungBearbeitenRequest
    {
        [Required]
        public string[] Empfaenger { get; set; }
        [Required]
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        [Required]
        public DateTime Zeitpunkt { get; set; }
    }
}
