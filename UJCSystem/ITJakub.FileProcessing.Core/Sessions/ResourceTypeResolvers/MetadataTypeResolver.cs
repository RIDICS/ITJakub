using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public class MetadataTypeResolver : ResourceTypeResolverBase
    {
        public MetadataTypeResolver(string[] fileExtensions)
            : base(fileExtensions)
        {
        }

        public override ResourceType ResolveResourceType
        {
            get { return ResourceType.UploadedMetadata; }
        }
    }
}