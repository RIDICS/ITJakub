using System.IO;
using System.Net;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Errors;
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

        public ProjectResourceManager(CommunicationProvider communicationProvider, AuthenticationManager authenticationManager,
            PermissionManager permissionManager, ForumSiteManager forumSiteManager, PortalTypeProvider portalTypeProvider)
        {
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
            m_permissionManager = permissionManager;
            m_forumSiteManager = forumSiteManager;
            m_portalTypeProvider = portalTypeProvider;
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
                try
                {
                    importResult = client.ProcessSession(sessionId, projectId, userId, comment, (ProjectTypeContract) m_portalTypeProvider.GetDefaultProjectType(), allAutoImportPermissions);
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
                m_forumSiteManager.CreateForums(importResult.ProjectId);
            }
            catch (ForumException e)
            {
                throw new MainServiceException(MainServiceErrorCode.ImportSucceedForumFailed, $"Import succeeded. Forum creation failed. {e.Message}", HttpStatusCode.BadRequest, new object[]{ e.Code});
            }
        }
    }
}
