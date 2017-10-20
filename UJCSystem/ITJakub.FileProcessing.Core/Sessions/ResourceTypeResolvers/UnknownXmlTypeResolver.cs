using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public class UnknownXmlTypeResolver : ResourceTypeResolverBase
    {
        public UnknownXmlTypeResolver(string[] fileExtensions)
            : base(fileExtensions)
        {
        }

        public override ResourceType ResolveResourceType
        {
            get { return ResourceType.UnknownXmlFile; }
        }
    }
}