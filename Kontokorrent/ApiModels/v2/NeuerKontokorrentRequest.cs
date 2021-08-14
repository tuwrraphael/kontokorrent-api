using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class NeuerKontokorrentRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [RegularExpression("^[a-z0-9]+$")]
        public string OeffentlicherName { get; set; }
        public NeuePerson[] Personen { get; set; }
    }
}
