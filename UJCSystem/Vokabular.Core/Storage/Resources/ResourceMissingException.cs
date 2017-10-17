using System;

namespace Vokabular.Core.Storage.Resources
{
    public class ResourceMissingException : Exception
    {
        public ResourceMissingException(string message) : base(message)
        {
        }
    }
}