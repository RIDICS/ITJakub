using System;

namespace ITJakub.MobileApps.MobileContracts
{
    public class MobileCommunicationException : Exception
    {
        public MobileCommunicationException(Exception exception) : base("Communication error.", exception)
        {
        }
    }
}
