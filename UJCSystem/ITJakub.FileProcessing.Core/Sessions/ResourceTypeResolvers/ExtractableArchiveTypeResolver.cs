using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public class ExtractableArchiveTypeResolver:ResourceTypeResolverBase
    {
        public ExtractableArchiveTypeResolver(string[] fileExtensions) : base(fileExtensions)
        {
        }

        public override ResourceType ResolveResourceType { get {return ResourceType.ExtractableArchive;} }
    }
}