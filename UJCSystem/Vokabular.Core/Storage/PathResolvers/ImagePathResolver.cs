using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class ImagePathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType()
        {
            return ResourceType.Image;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, "Images", fileName);
        }
    }
}