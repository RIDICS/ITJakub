using System.IO;
using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.Core.PathResolvers
{
    public class PagePathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType()
        {
            return ResourceType.Page;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, "Pages", fileName);
        }
    }
}