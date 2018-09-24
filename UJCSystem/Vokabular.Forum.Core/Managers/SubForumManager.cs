using System.Collections.Generic;
using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.ForumSite.Core.Managers
{
    public class SubForumManager
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ForumAccessRepository m_forumAccessRepository;

        public SubForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository,
            ForumAccessRepository forumAccessRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessRepository = forumAccessRepository;
        }

        public void CreateNewSubForum(CategoryContract category)
        {
            new CreateSubForumWork(m_forumRepository, m_categoryRepository, m_forumAccessRepository, category).Execute();
        }

        public void UpdateSubForum(CategoryContract updatedCategory, CategoryContract oldCategory)
        {
            new UpdateSubForumWork(m_forumRepository, m_categoryRepository, updatedCategory, oldCategory).Execute();
        }

        public void DeleteSubForum(int categoryId)
        {
            new DeleteSubForumWork(m_forumRepository, m_categoryRepository, m_forumAccessRepository, categoryId).Execute();
        }

        public void CreateVirtualForums(long projectId, IList<int> categoryIds, IList<int> oldCategoryIds)
        {
            new CreateVirtualForumsForCategories(m_forumRepository, m_forumAccessRepository, categoryIds, oldCategoryIds, projectId).Execute();
        }
    }
}