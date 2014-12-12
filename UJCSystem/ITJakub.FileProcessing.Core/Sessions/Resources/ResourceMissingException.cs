using System;

namespace ITJakub.FileProcessing.Core.Sessions.Resources
{
    public class ResourceMissingException : Exception
    {
        public ResourceMissingException(string message) : base(message)
        {
        }
    }
}