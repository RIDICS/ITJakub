using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public class SourceDocumentTypeResolver : ResourceTypeResolverBase
    {
        public SourceDocumentTypeResolver(string[] fileExtensions) : base(fileExtensions)
        {
        }

        public override ResourceType ResolveResourceType
        {
            get { return ResourceType.SourceDocument; }
        }
    }
}