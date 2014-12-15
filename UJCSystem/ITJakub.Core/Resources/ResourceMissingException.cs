using System;

namespace ITJakub.Core.Resources
{
    public class ResourceMissingException : Exception
    {
        public ResourceMissingException(string message) : base(message)
        {
        }
    }
}