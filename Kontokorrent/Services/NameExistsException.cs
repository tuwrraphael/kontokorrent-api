using System;
using System.Runtime.Serialization;

namespace Kontokorrent.Services
{
    [Serializable]
    internal class NameExistsException : Exception
    {
        public NameExistsException()
        {
        }

        public NameExistsException(string message) : base(message)
        {
        }

        public NameExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NameExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}