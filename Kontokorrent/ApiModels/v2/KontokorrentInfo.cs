namespace Kontokorrent.ApiModels.v2
{
    public class KontokorrentInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Person[] Personen { get; set; }
        public string OeffentlicherName { get; set; }
    }
}
