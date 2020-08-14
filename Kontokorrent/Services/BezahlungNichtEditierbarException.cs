using System;
using System.Runtime.Serialization;

namespace Kontokorrent.Services
{
    [Serializable]
    internal class BezahlungNichtEditierbarException : Exception
    {
        public BezahlungNichtEditierbarException()
        {
        }

        public BezahlungNichtEditierbarException(string message) : base(message)
        {
        }

        public BezahlungNichtEditierbarException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BezahlungNichtEditierbarException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}