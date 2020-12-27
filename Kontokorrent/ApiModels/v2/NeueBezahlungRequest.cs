using System;
using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class NeueBezahlungRequest
    {
        public string Id { get; set; }
        [Required]
        public string[] EmpfaengerIds { get; set; }
        [Required]
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        [Required]
        public DateTimeOffset Zeitpunkt { get; set; }
        public string BezahlendePersonId { get; set; }
    }
}
