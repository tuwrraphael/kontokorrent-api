namespace Kontokorrent.Impl.EF
{
    public class BenutzerKontokorrent
    {
        public string Id { get; set; }
        public string BenutzerId { get; set; }

        public string KontokorrentId { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
    }
}
