using System.IO;
using ITJakub.FileStorage.Resources;

namespace ITJakub.FileStorage.PathResolvers
{
    public class ImagePathResolver : IResourceTypePathResolver
    {
        public ResourceTypeEnum ResolvingResourceType()
        {
            return ResourceTypeEnum.Image;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, "Images", fileName);
        }
    }
}