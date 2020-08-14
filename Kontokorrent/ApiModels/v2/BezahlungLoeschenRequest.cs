using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class BezahlungLoeschenRequest
    {
        [Required]
        public string Id { get; set; }
    }
}
