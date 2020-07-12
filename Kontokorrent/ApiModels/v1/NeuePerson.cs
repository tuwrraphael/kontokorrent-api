using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v1
{
    public class NeuePerson
    {
        [Required]
        public string Name { get; set; }
    }
}
