using System;
using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class BezahlungBearbeitenRequest
    {
        [Required]
        public string[] EmpfaengerIds { get; set; }
        [Required]
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        [Required]
        public DateTimeOffset Zeitpunkt { get; set; }
    }
}
