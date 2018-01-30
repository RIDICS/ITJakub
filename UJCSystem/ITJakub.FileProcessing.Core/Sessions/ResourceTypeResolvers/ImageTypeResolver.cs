using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public class ImageTypeResolver : ResourceTypeResolverBase
    {
        public ImageTypeResolver(string[] fileExtensions)
            : base(fileExtensions)
        {
        }

        public override ResourceType ResolveResourceType
        {
            get { return ResourceType.Image; }
        }
    }
}