using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class BookPathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType => ResourceType.Book;

        public bool PreserveFileNameInStorage => true;

        public string ResolvePath(long projectId, string bookVersionExternalId, string fileName)
        {
            return Path.Combine(projectId.ToString(), bookVersionExternalId, fileName);
        }
    }
}