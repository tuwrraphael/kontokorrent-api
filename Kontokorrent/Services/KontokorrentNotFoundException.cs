using System;
using System.Runtime.Serialization;

namespace Kontokorrent.Services
{
    [Serializable]
    internal class KontokorrentNotFoundException : Exception
    {
        public KontokorrentNotFoundException()
        {
        }

        public KontokorrentNotFoundException(string message) : base(message)
        {
        }

        public KontokorrentNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KontokorrentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}