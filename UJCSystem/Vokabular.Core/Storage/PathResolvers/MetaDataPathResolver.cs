using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class ConvertedMetadataPathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType => ResourceType.ConvertedMetadata;

        public bool PreserveFileNameInStorage => true;

        public string ResolvePath(long projectId, string bookVersionExternalId, string fileName)
        {
            return Path.Combine(projectId.ToString(), bookVersionExternalId, fileName);
        }
    } 
    
    public class UploadedMetaDataPathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType => ResourceType.UploadedMetadata;

        public bool PreserveFileNameInStorage => true;

        public string ResolvePath(long projectId, string bookVersionExternalId, string fileName)
        {
            return Path.Combine(projectId.ToString(), bookVersionExternalId, fileName);
        }
    }
}