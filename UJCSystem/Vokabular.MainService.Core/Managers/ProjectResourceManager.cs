using System;
using System.IO;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Errors;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly PermissionManager m_permissionManager;
        private readonly ForumSiteManager m_forumSiteManager;

        public ProjectResourceManager(CommunicationProvider communicationProvider, AuthenticationManager authenticationManager, PermissionManager permissionManager, ForumSiteManager forumSiteManager)
        {
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
            m_permissionManager = permissionManager;
            m_forumSiteManager = forumSiteManager;
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

            ImportResultContract importResult;
            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                importResult = client.ProcessSession(sessionId, projectId, userId, comment, allAutoImportPermissions);
                if (!importResult.Success)
                {
                    throw new InvalidOperationException("Import failed");
                }
            }
   
            try
            {
                 m_forumSiteManager.CreateForums(importResult.ProjectId);
             }
             catch (ForumException e)
             {
                 throw new InvalidOperationException("Import succeeded. " + e.Message);
             }
        }
    }
}
