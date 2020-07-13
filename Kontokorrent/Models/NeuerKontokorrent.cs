namespace Kontokorrent.Models
{
    public class NeuerKontokorrent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OeffentlicherName { get; set; }
        public NeuePerson[] Personen { get; set; }
        public bool Privat { get; set; }
    }
}
