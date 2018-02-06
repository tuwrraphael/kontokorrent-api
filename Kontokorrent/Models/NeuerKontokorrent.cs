using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.Models
{
    public class NeuerKontokorrent
    {
        [Required]
        public string Secret { get; set; }
    }
}
