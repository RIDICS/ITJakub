using System;
using System.IO;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.MainService.Core.Communication;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly PermissionManager m_permissionManager;

        public ProjectResourceManager(CommunicationProvider communicationProvider, AuthenticationManager authenticationManager, PermissionManager permissionManager)
        {
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
            m_permissionManager = permissionManager;
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
            var userId = m_authenticationManager.GetCurrentUserId();
            var allAutoImportPermissions = m_permissionManager.GetAutoImportSpecialPermissions();

            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                var success = client.ProcessSession(sessionId, projectId, userId, comment, allAutoImportPermissions);
                if (!success)
                {
                    throw new InvalidOperationException("Import failed");
                }
            }
        }
    }
}
