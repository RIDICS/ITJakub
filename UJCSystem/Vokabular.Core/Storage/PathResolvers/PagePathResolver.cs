using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class PagePathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType => ResourceType.Page;

        public bool PreserveFileNameInStorage => true;

        public string ResolvePath(long projectId, string bookVersionExternalId, string fileName)
        {
            return Path.Combine(projectId.ToString(), bookVersionExternalId, "Pages", fileName);
        }
    }
}