using System;
using System.IO;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.MainService.Core.Communication;
using Vokabular.Shared;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly ForumSiteManager m_forumSiteManager;

        public ProjectResourceManager(CommunicationProvider communicationProvider, AuthorizationManager authorizationManager, ForumSiteManager forumSiteManager)
        {
            m_communicationProvider = communicationProvider;
            m_authorizationManager = authorizationManager;
            m_forumSiteManager = forumSiteManager;
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

        public void ProcessSessionAsImport(string sessionId, long? projectId, string comment, string hostUrl)
        {
            var permissionResult = m_authorizationManager.CheckUserCanUploadBook();

            ImportResult importResult;
            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                importResult = client.ProcessSession(sessionId, projectId, permissionResult.UserId, comment);
                if (!importResult.Success)
                {
                    throw new InvalidOperationException("Import failed");
                }
            }

            m_forumSiteManager.CreateForums(importResult, hostUrl);
        }
    }
}
