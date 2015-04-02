using System;

namespace ITJakub.MobileApps.Client.Shared.Communication
{
    public class ClientCommunicationException : System.Exception
    {
        public ClientCommunicationException()
        {
        }

        public ClientCommunicationException(string message) : base(message)
        {
        }

        public ClientCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ClientCommunicationException(Exception innerException) : base("Communication problem.", innerException)
        {
        }
    }
}