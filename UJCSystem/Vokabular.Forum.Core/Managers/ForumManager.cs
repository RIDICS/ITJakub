using System.Collections.Generic;
using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Managers
{
    public class ForumManager
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly TopicRepository m_topicRepository;
        private readonly MessageRepository m_messageRepository;
        private readonly UserRepository m_userRepository;
        private readonly ForumAccessRepository m_forumAccessRepository;
        private readonly AccessMaskRepository m_accessMaskRepository;
        private readonly GroupRepository m_groupRepository;

        public ForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            MessageRepository messageRepository, UserRepository userRepository, ForumAccessRepository forumAccessRepository,
            AccessMaskRepository accessMaskRepository, GroupRepository groupRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_userRepository = userRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_accessMaskRepository = accessMaskRepository;
            m_groupRepository = groupRepository;
        }

        public Forum GetForum(int forumId)
        {
            return m_forumRepository.InvokeUnitOfWork(x => x.FindById<Forum>(forumId));
        }

        public long CreateNewForum(ProjectDetailContract project, short[] bookTypeIds, UserDetailContract user)
        {
            var work = new CreateForumWork(m_forumRepository, m_categoryRepository, m_topicRepository, m_messageRepository,
                m_userRepository, m_forumAccessRepository, m_accessMaskRepository, m_groupRepository, project, bookTypeIds, user);
            var resultId = work.Execute();
            return resultId;
        }
    }
}