using System;
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
        private readonly CategoryRepository m_categoryRepository;
        private readonly UserManager m_userManager;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly ForumManager m_forumManager;
        private readonly SubForumManager m_subForumManager;

        public ForumSiteManager(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            CategoryRepository categoryRepository, UserManager userManager, AuthorizationManager authorizationManager,
            ForumManager forumManager, SubForumManager subForumManager)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_userManager = userManager;
            m_authorizationManager = authorizationManager;
            m_forumManager = forumManager;
            m_subForumManager = subForumManager;
            m_categoryRepository = categoryRepository;
        }

        public void CreateForums(ImportResult importResult)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, importResult.ProjectId, true, true, false, true);
            Project project = work.Execute();

            if (project == null)
            {
                throw new InvalidOperationException("Create of forum failed. Import was successful.");
            }

            ProjectDetailContract projectDetailContract = Mapper.Map<ProjectDetailContract>(project);
            projectDetailContract.PageCount = work.GetPageCount();
            short[] bookTypeIds = project.LatestPublishedSnapshot.BookTypes.Select(x => (short) x.Type).ToArray();
            UserDetailContract user = m_userManager.GetUserDetail(m_authorizationManager.GetCurrentUserId());

            m_forumManager.CreateNewForum(projectDetailContract, bookTypeIds, user);
        }

        public void CreateCategory(CategoryContract category, int categoryId)
        {
            category.Id = categoryId;
            m_subForumManager.CreateNewSubForum(category);
        }

        public void UpdateCategory(CategoryContract updatedCategory, int categoryId)
        {
            var category = m_categoryRepository.FindById<Category>(categoryId);
            m_subForumManager.UpdateSubForum(updatedCategory, Mapper.Map<CategoryContract>(category));
        }
    }
}