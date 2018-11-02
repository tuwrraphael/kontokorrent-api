using System;
using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.Models
{
    public class GeaenderteBezahlung
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
