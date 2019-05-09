using System;
using System.Runtime.Serialization;

namespace Vokabular.FulltextService.Core.Helpers
{
    public class FulltextDatabaseException : Exception
    {
        public FulltextDatabaseException()
        {
        }

        public FulltextDatabaseException(string message) : base(message)
        {
        }

        public FulltextDatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FulltextDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}