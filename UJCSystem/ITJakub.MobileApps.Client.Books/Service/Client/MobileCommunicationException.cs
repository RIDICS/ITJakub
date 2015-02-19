using System;

namespace ITJakub.MobileApps.Client.Books.Service.Client
{
    public class MobileCommunicationException : Exception
    {
        public MobileCommunicationException(Exception exception) : base("Communication error.", exception)
        {
        }
    }
}
