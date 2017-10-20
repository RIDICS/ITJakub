using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
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