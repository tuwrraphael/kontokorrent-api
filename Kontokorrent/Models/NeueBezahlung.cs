using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Models
{
    public class NeueBezahlung
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
}
