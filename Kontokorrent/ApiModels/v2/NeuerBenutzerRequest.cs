using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class NeuerBenutzerRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Secret { get; set; }

        public string OeffentlicherName { get; set; }
        public string EinladungsCode { get; set; }
    }
}
