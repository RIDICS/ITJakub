using System;

namespace ITJakub.MobileApps.Client.Books.Service.Client
{
    public class NotFoundException : Exception
    {
        public NotFoundException(Exception exception) : base("Resource not found.", exception)
        {
        }
    }
}