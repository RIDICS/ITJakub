using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ForumSite.Core.Managers;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared;

namespace Vokabular.MainService.Core.Managers
{
    public class ForumSiteManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly UserManager m_userManager;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly ForumManager m_forumManager;

        public ForumSiteManager(ProjectRepository projectRepository, MetadataRepository metadataRepository, UserManager userManager, AuthorizationManager authorizationManager, ForumManager forumManager)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_userManager = userManager;
            m_authorizationManager = authorizationManager;
            m_forumManager = forumManager;
        }

        public void CreateForums(ImportResult importResult)
        {
            //TODO all requests in one unit of work
            //TODO get Project & BookTypes
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, importResult.ProjectId, false, false, false, true);
            Project project = work.Execute();
            
            /*if (project == null) //TODO exception?
             {
                 return null;
             }*/

            ProjectDetailContract projectDetailContract = Mapper.Map<ProjectDetailContract>(project);
            short[] bookTypeIds = project.LatestPublishedSnapshot.BookTypes.Select(x => (short)x.Type).ToArray();
            UserDetailContract user = m_userManager.GetUserDetail(m_authorizationManager.GetCurrentUserId());

            m_forumManager.CreateNewForum(projectDetailContract, bookTypeIds, user);
        }
    }
}
