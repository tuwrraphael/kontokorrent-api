using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.Models
{
    public class NeuePerson
    {
        [Required]
        public string Name { get; set; }
    }
}
