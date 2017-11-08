using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public abstract class ResourceTypeResolverBase
    {
        protected ResourceTypeResolverBase(string[] fileExtensions)
        {
            FileExtensions = fileExtensions;
        }

        public abstract ResourceType ResolveResourceType { get; }

        public string[] FileExtensions { get; private set; }
    }
}