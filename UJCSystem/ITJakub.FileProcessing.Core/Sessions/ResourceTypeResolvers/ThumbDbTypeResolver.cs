using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public class ThumbDbTypeResolver : ResourceTypeResolverBase
    {
        public ThumbDbTypeResolver(string[] fileExtensions)
            : base(fileExtensions)
        {
        }

        public override ResourceType ResolveResourceType
        {
            get { return ResourceType.ThumbDbFile; }
        }
    }
}