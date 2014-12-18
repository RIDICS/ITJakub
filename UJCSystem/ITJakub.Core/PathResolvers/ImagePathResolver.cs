using System.IO;
using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.Core.PathResolvers
{
    public class ImagePathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType()
        {
            return ResourceType.Image;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, "Images", fileName);
        }
    }
}