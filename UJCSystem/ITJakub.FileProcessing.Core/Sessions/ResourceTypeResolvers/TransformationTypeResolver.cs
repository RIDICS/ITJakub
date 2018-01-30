using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers
{
    public class TransformationTypeResolver : ResourceTypeResolverBase
    {
        public TransformationTypeResolver(string[] fileExtensions)
            : base(fileExtensions)
        {
        }

        public override ResourceType ResolveResourceType
        {
            get { return ResourceType.Transformation; }
        }
    }
}