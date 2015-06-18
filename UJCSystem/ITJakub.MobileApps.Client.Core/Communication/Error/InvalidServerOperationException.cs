using System;

namespace ITJakub.MobileApps.Client.Core.Communication.Error
{
    public class InvalidServerOperationException : Exception
    {
        public InvalidServerOperationException()
        {
        }

        public InvalidServerOperationException(string message) : base(message)
        {
        }

        public InvalidServerOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidServerOperationException(Exception innerException) : base("Invalid server operation", innerException)
        {
        }
    }
}