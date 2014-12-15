using System.IO;
using ITJakub.FileStorage.Resources;

namespace ITJakub.FileStorage.PathResolvers
{
    public class BookPathResolver : IResourceTypePathResolver
    {
        public ResourceTypeEnum ResolvingResourceType()
        {
            return ResourceTypeEnum.Book;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, fileName);
        }
    }
}