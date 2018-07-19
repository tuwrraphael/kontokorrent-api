using System.Collections.Generic;

namespace Kontokorrent.Impl.EF
{
    public class Kontokorrent
    {
        public string Id { get; set; }
        public string Secret { get; set; }

        public List<Person> Personen { get; set; }
        public List<Bezahlung> Bezahlungen { get; set; }
    }
}
