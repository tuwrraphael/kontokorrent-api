using System;
using System.Runtime.Serialization;

namespace Kontokorrent.Services
{
    [Serializable]
    internal class PersonExistiertNichtException : Exception
    {
        public PersonExistiertNichtException()
        {
        }

        public PersonExistiertNichtException(string message) : base(message)
        {
        }

        public PersonExistiertNichtException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PersonExistiertNichtException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}