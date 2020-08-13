using System;

namespace Kontokorrent.Services
{
    [Serializable]
    internal class BezahlungEditException : Exception
    {
        public const int NichtEditierbar = 100;
        public const int NichtGefunden = 101;
        public BezahlungEditException(int grund)
        {
            Grund = grund;
        }

        public int Grund { get; }
    }
}