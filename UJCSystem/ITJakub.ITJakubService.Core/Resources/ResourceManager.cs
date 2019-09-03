﻿using ITJakub.FileProcessing.DataContracts;

namespace ITJakub.ITJakubService.Core.Resources
{
    public class ResourceManager
    {
        private readonly FileProcessingServiceClient m_resourceClient;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly UserManager m_userManager;

        public ResourceManager(FileProcessingServiceClient resourceClient, AuthorizationManager authorizationManager, UserManager userManager)
        {
            m_resourceClient = resourceClient;
            m_authorizationManager = authorizationManager;
            m_userManager = userManager;
        }

        public void AddResource(DataContracts.UploadResourceContract resourceInfoSkeleton)
        {
            m_authorizationManager.CheckUserCanUploadBook();
            m_resourceClient.AddResource(new UploadResourceContract
            {
                SessionId = resourceInfoSkeleton.SessionId,
                FileName = resourceInfoSkeleton.FileName,
                Data = resourceInfoSkeleton.Data,
            });
        }

        public bool ProcessSession(string resourceSessionId, long? projectId, string uploadMessage)
        {
            var userId = m_userManager.GetCurrentUser().Id;
            m_authorizationManager.CheckUserCanUploadBook();
            return m_resourceClient.ProcessSession(resourceSessionId, projectId, userId, uploadMessage, ProjectTypeContract.Research, FulltextStoreTypeContract.ExistDb, null /*Service is obsolete, so correct fix is not required*/).Success;
        }
    }
}
