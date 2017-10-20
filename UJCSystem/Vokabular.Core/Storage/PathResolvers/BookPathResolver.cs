using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class BookPathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType()
        {
            return ResourceType.Book;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, bookVersionId, fileName);
        }
    }
}