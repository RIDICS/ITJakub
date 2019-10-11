using System.IO;
using System.Net;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Errors;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.DataContracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly PermissionManager m_permissionManager;
        private readonly ForumSiteManager m_forumSiteManager;
        private readonly PortalTypeProvider m_portalTypeProvider;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;

        public ProjectResourceManager(CommunicationProvider communicationProvider, AuthenticationManager authenticationManager,
            PermissionManager permissionManager, ForumSiteManager forumSiteManager, PortalTypeProvider portalTypeProvider, FulltextStorageProvider fulltextStorageProvider)
        {
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
            m_permissionManager = permissionManager;
            m_forumSiteManager = forumSiteManager;
            m_portalTypeProvider = portalTypeProvider;
            m_fulltextStorageProvider = fulltextStorageProvider;
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
            var projectType = m_portalTypeProvider.GetDefaultProjectType();
            var fulltextStorageType = m_fulltextStorageProvider.GetStorageType((ProjectTypeEnum) projectType);

            ImportResultContract importResult;
            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                try
                {
                    importResult = client.ProcessSession(sessionId, projectId, userId, comment, (ProjectTypeContract) projectType, (FulltextStoreTypeContract) fulltextStorageType, allAutoImportPermissions);
                    if (!importResult.Success)
                    {
                        throw new MainServiceException(MainServiceErrorCode.ImportFailed, "Import failed");
                    }
                }
                catch (FileProcessingImportFailedException exception)
                {
                    throw new MainServiceException(MainServiceErrorCode.ImportFailedWithError, $"Import failed with error: {exception.InnerException?.Message}", descriptionParams: exception.InnerException?.Message);
                }
            }

            try
            {
                m_forumSiteManager.CreateOrUpdateForums(importResult.ProjectId);
            }
            catch (ForumException e)
            {
                throw new MainServiceException(MainServiceErrorCode.ImportSucceedForumFailed, $"Import succeeded. Forum creation failed. {e.Message}", HttpStatusCode.BadRequest, new object[]{ e.Code});
            }
        }
    }
}
