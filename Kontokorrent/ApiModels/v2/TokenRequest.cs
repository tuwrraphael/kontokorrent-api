using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class TokenRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Secret { get; set; }
    }
}
