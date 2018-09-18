using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ForumSite.Core.Managers;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectResourceManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly ProjectManager m_projectManager;
        private readonly ForumManager m_forumManager;
        private readonly UserManager m_userManager;
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;

        public ProjectResourceManager(CommunicationProvider communicationProvider, AuthorizationManager authorizationManager, ProjectManager projectManager, ForumManager forumManager, UserManager userManager, ProjectRepository projectRepository, MetadataRepository metadataRepository)
        {
            m_communicationProvider = communicationProvider;
            m_authorizationManager = authorizationManager;
            m_projectManager = projectManager;
            m_forumManager = forumManager;
            m_userManager = userManager;
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
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

            ImportResult importResult;
            using (var client = m_communicationProvider.GetFileProcessingClient())
            {
                importResult = client.ProcessSession(sessionId, projectId, permissionResult.UserId, comment);
                if (!importResult.Success)
                {
                    throw new InvalidOperationException("Import failed");
                }
            }


            //-----new class - manager which calls createNewForum
            //all requests in one unit of work

            //TODO get Project & BookTypes
            // ProjectDetailContract project =  m_projectManager.GetProject(importResult.ProjectId,false, false, false);


            //TODO move to manager
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, importResult.ProjectId, true, true, true, true);
            Project project = work.Execute();

            /* if (project == null) //TODO 
             {
                 return null;
             }*/


            //TODO create forum
            UserDetailContract user = m_userManager.GetUserDetail(m_authorizationManager.GetCurrentUserId());
            //m_forumManager.CreateNewForum(project, work.BookTypes, user);
            //-----
        }
    }
}
