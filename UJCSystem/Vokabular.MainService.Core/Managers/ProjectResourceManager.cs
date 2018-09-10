using System;
using System.IO;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.MainService.Core.Communication;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectResourceManager(CommunicationProvider communicationProvider, AuthorizationManager authorizationManager)
        {
            m_communicationProvider = communicationProvider;
            m_authorizationManager = authorizationManager;
        }

        public void UploadResource(string sessionId, Stream data, string fileName)
        {
            m_authorizationManager.CheckUserCanUploadBook();

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
            var permissionResult = m_authorizationManager.CheckUserCanUploadBook();

            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                var success = client.ProcessSession(sessionId, projectId, permissionResult.UserId, comment);
                if (!success)
                {
                    throw new InvalidOperationException("Import failed");
                }
            }

            //TODO return object with id from client.ProcessSession
            //TODO create forum
        }
    }
}
