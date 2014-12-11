using System.IO;
using Castle.Windsor;
using ITJakub.FileProcessing.Core.Sessions;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Service
{

    public interface IFileProcessingServiceLocal : IFileProcessingService
    {
        
    }


    public class FileProcessingService : IFileProcessingService
    {
        private readonly ResourceSessionManager m_sessionManager;

        public FileProcessingService(ResourceSessionManager sessionManager)
        {
            m_sessionManager = sessionManager;
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_sessionManager.AddResource(resourceInfoSkeleton);

        }

        public bool ProcessSession(string sessionId)
        {
            return m_sessionManager.ProcessSession(sessionId);
        }
    }
}