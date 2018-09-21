using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Managers
{
    public class SubForumManager
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ForumAccessRepository m_forumAccessRepository;
        private readonly AccessMaskRepository m_accessMaskRepository;
        private readonly GroupRepository m_groupRepository;

        public SubForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository, ForumAccessRepository forumAccessRepository,
            AccessMaskRepository accessMaskRepository, GroupRepository groupRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_accessMaskRepository = accessMaskRepository;
            m_groupRepository = groupRepository;
        }

        public int CreateNewSubForum(CategoryContract category, UserDetailContract user)
        {
            var work = new CreateSubForumWork(m_forumRepository, m_categoryRepository, m_forumAccessRepository, m_accessMaskRepository, m_groupRepository, category);
            var resultId = work.Execute();
            return resultId;
        }
    }
}