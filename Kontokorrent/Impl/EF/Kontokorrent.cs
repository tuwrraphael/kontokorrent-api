using System.Collections.Generic;

namespace Kontokorrent.Impl.EF
{
    public class Kontokorrent
    {
        public string Id { get; set; }
        public string OeffentlicherName { get; set; }
        public List<Person> Personen { get; set; }
        public List<Bezahlung> Bezahlungen { get; set; }
        public List<BenutzerKontokorrent> Benutzer { get; set; }
        public string Name { get; set; }
        public bool Privat { get; set; }
        public List<EinladungsCode> EinladungsCodes { get; set; }
    }
}
