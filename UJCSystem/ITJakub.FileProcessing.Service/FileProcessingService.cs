using System.Collections.Generic;
using ITJakub.FileProcessing.Core.Sessions;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.Shared;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly ResourceSessionManager m_sessionManager = Container.Current.Resolve<ResourceSessionManager>();

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_sessionManager.AddResource(resourceInfoSkeleton.SessionId, resourceInfoSkeleton.FileName, resourceInfoSkeleton.Data);
        }

        public ImportResult ProcessSession(string sessionId, long? projectId, int userId, string uploadMessage, IList<PermissionFromAuthContract> autoImportPermissions)
        {
            return m_sessionManager.ProcessSession(sessionId, projectId, userId, uploadMessage, autoImportPermissions);
        }
    }
}