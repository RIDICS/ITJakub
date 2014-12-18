using System.IO;
using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.Core.PathResolvers
{
    public class TransformationPathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType()
        {
            return ResourceType.Transformation;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, "Transformations", fileName);
        }
    }
}