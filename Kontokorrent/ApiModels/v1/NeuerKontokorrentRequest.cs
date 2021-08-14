using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v1
{
    public class NeuerKontokorrentRequest
    {
        [Required]
        [RegularExpression("^[A-Za-z0-9]+$")]
        public string Secret { get; set; }

        public NeuePerson[] Personen { get; set; }
    }
}
