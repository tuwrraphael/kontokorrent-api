using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v1
{
    public class NeuerKontokorrentRequest
    {
        [Required]
        public string Secret { get; set; }

        public NeuePerson[] Personen { get; set; }
    }
}
