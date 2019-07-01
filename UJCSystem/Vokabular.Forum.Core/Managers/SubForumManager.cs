using System.Collections.Generic;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.Core.Works.Subworks;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.ForumSite.Core.Managers
{
    public class SubForumManager
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ForumAccessSubwork m_forumAccessSubwork;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;

        public SubForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository,
            ForumAccessSubwork forumAccessSubwork, ForumSiteUrlHelper forumSiteUrlHelper)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessSubwork = forumAccessSubwork;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
        }

        public void CreateNewSubForum(CategoryContract category)
        {
            new CreateSubForumWork(m_forumRepository, m_categoryRepository, m_forumAccessSubwork, category).Execute();
        }

        public void UpdateSubForum(CategoryContract updatedCategory, CategoryContract oldCategory)
        {
            new UpdateSubForumWork(m_forumRepository, m_categoryRepository, updatedCategory, oldCategory).Execute();
        }

        public void DeleteSubForum(int categoryId)
        {
            new DeleteSubForumWork(m_forumRepository, m_categoryRepository, categoryId).Execute();
        }

        public void CreateVirtualForums(long projectId, IList<int> categoryIds, IList<int> oldCategoryIds)
        {
            new CreateVirtualForumsForCategories(m_forumRepository, m_forumAccessSubwork, m_forumSiteUrlHelper, categoryIds, oldCategoryIds, projectId).Execute();
        }
    }
}