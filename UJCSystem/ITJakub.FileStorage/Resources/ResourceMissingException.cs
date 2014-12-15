using System;

namespace ITJakub.FileStorage.Resources
{
    public class ResourceMissingException : Exception
    {
        public ResourceMissingException(string message) : base(message)
        {
        }
    }
}