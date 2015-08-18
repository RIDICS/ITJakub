using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.Core.Resources
{
    public class Resource
    {
        public string FullPath { get; set; }

        public string FileName { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}