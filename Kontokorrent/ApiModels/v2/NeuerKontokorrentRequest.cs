using System.ComponentModel.DataAnnotations;

namespace Kontokorrent.ApiModels.v2
{
    public class NeuerKontokorrentRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string OeffentlicherName { get; set; }
        public NeuePerson[] Personen { get; set; }
    }

    public class KontokorrentListenEintrag
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
