using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Managers;
using Vokabular.MainService.Core.Errors;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ForumSiteManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly ForumManager m_forumManager;
        private readonly SubForumManager m_subForumManager;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;

        public ForumSiteManager(ProjectRepository projectRepository, MetadataRepository metadataRepository, ForumManager forumManager,
            SubForumManager subForumManager, ForumSiteUrlHelper forumSiteUrlHelper)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_forumManager = forumManager;
            m_subForumManager = subForumManager;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
        }

        public int CreateForums(long projectId)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, projectId, true, true, false, true);
            var project = work.Execute();

            if (project == null)
            {
                throw new ForumException("Create of forum failed. The project does not exist.");
            }

            var projectDetailContract = Mapper.Map<ProjectDetailContract>(project);
            projectDetailContract.PageCount = work.GetPageCount();
            var bookTypeIds = project.LatestPublishedSnapshot.BookTypes.Select(x => (short) x.Type).ToArray();

            if (project.ForumId != null)
            {
                throw new ForumException("Create of forum failed. The forum is already created.");
            }

            var forum = m_forumManager.GetForumByExternalId(project.Id);

            if (forum == null)
            {
                //Create forum
                var forumId = m_forumManager.CreateNewForum(projectDetailContract, bookTypeIds);
                new SetForumIdToProjectWork(m_projectRepository, projectId, forumId).Execute();
                return forumId;
            }

            //Forum is created, but not connect with project -> set ForumId to Project
            new SetForumIdToProjectWork(m_projectRepository, projectId, forum.ForumID).Execute();
            return forum.ForumID;
        }

        public void UpdateForums(long projectId, string hostUrl)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, projectId, true, true, false, true);
            var project = work.Execute();

            if (project == null)
            {
                throw new ForumException("Update of forum failed. Project does not exist.");
            }

            var projectDetailContract = Mapper.Map<ProjectDetailContract>(project);
            projectDetailContract.PageCount = work.GetPageCount();
            var bookTypeIds = project.LatestPublishedSnapshot.BookTypes.Select(x => (short) x.Type).ToArray();

            if (project.ForumId != null)
            {
                m_forumManager.UpdateForum(projectDetailContract, bookTypeIds, hostUrl);
            }
        }


        public ForumContract GetForum(long projectId)
        {
            var forum = m_forumManager.GetForumByExternalId(projectId);
            if (forum == null)
            {
                return null;
            }

            var result = Mapper.Map<ForumContract>(forum);
            result.Url = m_forumSiteUrlHelper.GetTopicsUrl(result.Id);
            return result;
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