namespace Kontokorrent.Impl.EF
{
    public class EmfpaengerInBezahlung
    {
        public int Id { get; set; }
        public string EmpfaengerId { get; set; }
        public Person Empfaenger { get; set; }
        public string BezahlungId { get; set; }
        public Bezahlung Bezahlung { get; set; }
    }
}
