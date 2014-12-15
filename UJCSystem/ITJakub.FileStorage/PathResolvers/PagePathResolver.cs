using System.IO;
using ITJakub.FileStorage.Resources;

namespace ITJakub.FileStorage.PathResolvers
{
    public class PagePathResolver : IResourceTypePathResolver
    {
        public ResourceTypeEnum ResolvingResourceType()
        {
            return ResourceTypeEnum.Page;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, "Pages", fileName);
        }
    }
}