using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class ImagePathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType => ResourceType.Image;

        public bool PreserveFileNameInStorage => false;

        public string ResolvePath(long projectId, string bookVersionExternalId, string fileName)
        {
            return Path.Combine(projectId.ToString(), "Images", fileName);
        }
    }
}