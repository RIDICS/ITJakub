using ITJakub.FileProcessing.DataContracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService.Core.Resources
{
    public class ResourceManager
    {
        private readonly FileProcessingServiceClient m_resourceClient;
        private readonly AuthorizationManager m_authorizationManager;

        public ResourceManager(FileProcessingServiceClient resourceClient, AuthorizationManager authorizationManager)
        {
            m_resourceClient = resourceClient;
            m_authorizationManager = authorizationManager;
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_authorizationManager.CheckUserCanUploadBook();
            m_resourceClient.AddResource(resourceInfoSkeleton);
        }

        public bool ProcessSession(string resourceSessionId, long projectId, string uploadMessage)
        {
            m_authorizationManager.CheckUserCanUploadBook();
            return m_resourceClient.ProcessSession(resourceSessionId, projectId, uploadMessage);
        }
    }
}
