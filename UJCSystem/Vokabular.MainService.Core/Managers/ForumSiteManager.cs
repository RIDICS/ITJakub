using System;
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
        private readonly SubForumManager m_subForumManager;

        public ForumSiteManager(ProjectRepository projectRepository, MetadataRepository metadataRepository, UserManager userManager,
            AuthorizationManager authorizationManager, ForumManager forumManager, SubForumManager subForumManager)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_userManager = userManager;
            m_authorizationManager = authorizationManager;
            m_forumManager = forumManager;
            m_subForumManager = subForumManager;
        }

        public void CreateForums(ImportResult importResult, string hostUrl)
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

            if (project.ForumId == null)
            {
                int forumId = m_forumManager.CreateNewForum(projectDetailContract, bookTypeIds, user, hostUrl);
                new SetForumIdToProjectWork(m_projectRepository, importResult.ProjectId, forumId).Execute();
            }
            else
            {
                m_forumManager.UpdateForum(projectDetailContract, bookTypeIds, user, hostUrl);
            }
        }

        public void CreateCategory(CategoryContract category, int categoryId)
        {
            category.Id = categoryId;
            m_subForumManager.CreateNewSubForum(category);
        }

        public void UpdateCategory(CategoryContract updatedCategory, CategoryContract oldCategory)
        {
            m_subForumManager.UpdateSubForum(updatedCategory, oldCategory);
        }

        public void DeleteCategory(int categoryId)
        {
            m_subForumManager.DeleteSubForum(categoryId);
        }

        public void SetCategoriesToForum(long projectId, IList<int> categoryIds, IList<int> oldCategoryIds)
        {
            m_subForumManager.CreateVirtualForums(projectId, categoryIds, oldCategoryIds);
        }
    }
}