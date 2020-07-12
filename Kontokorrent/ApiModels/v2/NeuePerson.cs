using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class NeuePerson
    {
        [Required]
        public string Name { get; set; }
    }
}
