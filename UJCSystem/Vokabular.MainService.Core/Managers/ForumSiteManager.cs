using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ForumSite.Core.Managers;
using Vokabular.MainService.Core.Errors;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared;

namespace Vokabular.MainService.Core.Managers
{
    public class ForumSiteManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly ForumManager m_forumManager;
        private readonly SubForumManager m_subForumManager;

        public ForumSiteManager(ProjectRepository projectRepository, MetadataRepository metadataRepository, ForumManager forumManager,
            SubForumManager subForumManager)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_forumManager = forumManager;
            m_subForumManager = subForumManager;
        }

        public void CreateForums(ImportResult importResult, string hostUrl)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, importResult.ProjectId, true, true, false, true);
            Project project = work.Execute();

            if (project == null)
            {
                throw new ForumException("Create of forum failed. Project does not exist.");
            }

            ProjectDetailContract projectDetailContract = Mapper.Map<ProjectDetailContract>(project);
            projectDetailContract.PageCount = work.GetPageCount();
            short[] bookTypeIds = project.LatestPublishedSnapshot.BookTypes.Select(x => (short) x.Type).ToArray();

            if (project.ForumId == null)
            {                
                var forum = m_forumManager.GetForumByExternalId(project.Id);

                if (forum == null)
                {
                    //Create forum
                    int forumId = m_forumManager.CreateNewForum(projectDetailContract, bookTypeIds, hostUrl);
                    new SetForumIdToProjectWork(m_projectRepository, importResult.ProjectId, forumId).Execute();
                }
                else
                {
                    //Forum is created, but not connect with project -> set ForumId to Project
                    new SetForumIdToProjectWork(m_projectRepository, importResult.ProjectId, forum.ForumID).Execute();
                }
            }
            else
            {
                m_forumManager.UpdateForum(projectDetailContract, bookTypeIds, hostUrl);
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