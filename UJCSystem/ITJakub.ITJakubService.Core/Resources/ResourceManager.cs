using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService.Core.Resources
{
    public class ResourceManager
    {
        private readonly FileProcessingServiceClient m_resourceClient;

        public ResourceManager(FileProcessingServiceClient resourceClient)
        {
            m_resourceClient = resourceClient;
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_resourceClient.AddResource(resourceInfoSkeleton);
        }

        public bool ProcessSession(string resourceSessionId, string uploadMessage)
        {
            return m_resourceClient.ProcessSession(resourceSessionId, uploadMessage);
        }
    }
}
