using System.IO;
using Castle.Windsor;
using ITJakub.FileProcessing.Core.Sessions;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly ResourceSessionManager m_sessionManager;

        public FileProcessingService(ResourceSessionManager sessionManager)
        {
            m_sessionManager = sessionManager;
        }

        public void AddResource(string sessionId, string fileName, Stream dataStream)
        {
            m_sessionManager.AddResource(sessionId, fileName, dataStream);

        }

        public bool ProcessSession(string sessionId)
        {
            return m_sessionManager.ProcessSession(sessionId);
        }
    }
}