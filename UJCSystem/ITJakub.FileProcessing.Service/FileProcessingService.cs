using ITJakub.FileProcessing.Core.Sessions;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly ResourceSessionManager m_sessionManager = Container.Current.Resolve<ResourceSessionManager>();

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_sessionManager.AddResource(resourceInfoSkeleton);

        }

        public bool ProcessSession(string sessionId, long? projectId, int userId, string uploadMessage)
        {
            return m_sessionManager.ProcessSession(sessionId, projectId, userId, uploadMessage);
        }
    }
}