﻿namespace Kontokorrent.ApiModels.v2
{
    public class Aktion
    {
        public int LaufendeNummer { get; set; }
        public Bezahlung Bezahlung { get; set; }
        public string BearbeiteteBezahlungId { get; set; }
        public string GeloeschteBezahlungId { get; set; }
    }
}
