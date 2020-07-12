using System;

namespace Kontokorrent.Impl.EF
{
    public class EinladungsCode
    {
        public string Id { get; set; }
        public DateTime GueltigBis { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
        public string KontokorrentId { get; set; }
    }
}
