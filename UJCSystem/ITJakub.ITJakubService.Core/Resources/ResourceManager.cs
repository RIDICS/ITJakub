using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.ITJakubService.Core.Resources
{
    public class ResourceManager
    {
        private readonly FileProcessingServiceClient m_resourceClient;

        public ResourceManager(FileProcessingServiceClient resourceClient)
        {
            m_resourceClient = resourceClient;
        }

        public void AddResource(string resourceSessionId, string fileName, Stream dataStream)
        {         
            m_resourceClient.AddResource(resourceSessionId, fileName, dataStream);
        }

        public bool ProcessSession(string resourceSessionId)
        {
            return m_resourceClient.ProcessSession(resourceSessionId);
        }
    }
}
