using System.IO;
using ITJakub.FileStorage.Resources;

namespace ITJakub.FileStorage.PathResolvers
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