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
        private readonly AccessMaskRepository m_accessMaskRepository;
        private readonly GroupRepository m_groupRepository;

        public SubForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository,
            ForumAccessRepository forumAccessRepository,
            AccessMaskRepository accessMaskRepository, GroupRepository groupRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_accessMaskRepository = accessMaskRepository;
            m_groupRepository = groupRepository;
        }

        public void CreateNewSubForum(CategoryContract category)
        {
            var work = new CreateSubForumWork(m_forumRepository, m_categoryRepository, m_forumAccessRepository, m_accessMaskRepository,
                m_groupRepository, category);
            work.Execute();
        }

        public void UpdateSubForum(CategoryContract updatedCategory, CategoryContract oldCategory)
        {
            var work = new UpdateSubForumWork(m_forumRepository, m_categoryRepository, updatedCategory, oldCategory);
            work.Execute();
        }

        public void DeleteSubForum(int categoryId)
        {
            var work = new DeleteSubForumWork(m_forumRepository, m_categoryRepository, m_forumAccessRepository, categoryId);
            work.Execute();
        }
    }
}