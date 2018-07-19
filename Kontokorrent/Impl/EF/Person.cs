using System.Collections.Generic;

namespace Kontokorrent.Impl.EF
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string KontokorrentId { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
        public List<Bezahlung> Bezahlungen { get; set; }
        public List<EmfpaengerInBezahlung> EmfpaengerIn { get; set; }
    }
}
