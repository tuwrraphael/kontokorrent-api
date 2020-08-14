using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels
{
    public class NeuePersonRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
