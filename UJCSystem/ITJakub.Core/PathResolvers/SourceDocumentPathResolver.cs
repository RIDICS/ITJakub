using System.IO;
using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.Core.PathResolvers
{
    public class SourceDocumentPathResolver : IResourceTypePathResolver
    {
        public ResourceTypeEnum ResolvingResourceType()
        {
            return ResourceTypeEnum.SourceDocument;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, fileName);
        }
    }
}