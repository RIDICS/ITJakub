using System;
using System.IO;
using ITJakub.Shared.Contracts.Resources;
using Vokabular.MainService.Core.Communication;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly UserManager m_userManager;

        public ProjectResourceManager(CommunicationProvider communicationProvider, UserManager userManager)
        {
            m_communicationProvider = communicationProvider;
            m_userManager = userManager;
        }

        public void UploadResource(string sessionId, Stream data, string fileName)
        {
            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                var resourceInfo = new UploadResourceContract
                {
                    SessionId = sessionId,
                    Data = data,
                    FileName = fileName
                };
                client.AddResource(resourceInfo);
            }
        }

        public void ProcessSessionAsImport(string sessionId, long? projectId, string comment)
        {
            var userId = m_userManager.GetCurrentUserId();
            
            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                var success = client.ProcessSession(sessionId, projectId, userId, comment);
                if (!success)
                {
                    throw new InvalidOperationException("Import failed");
                }
            }
        }
    }
}
