using System;
using System.Data;

namespace ITJakub.MobileApps.DataEntities
{
    public class CreateEntityFailedException : Exception
    {
        public CreateEntityFailedException(string message) : base(message)
        {
        }

        public CreateEntityFailedException()
        {
        }

        public CreateEntityFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
