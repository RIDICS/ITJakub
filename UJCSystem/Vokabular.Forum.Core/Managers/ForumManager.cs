using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;

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
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;

        public ForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            MessageRepository messageRepository, UserRepository userRepository, ForumAccessRepository forumAccessRepository, ForumSiteUrlHelper forumSiteUrlHelper)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_userRepository = userRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
        } 

        public int CreateNewForum(ProjectDetailContract project, short[] bookTypeIds, UserDetailContract user, string hostUrl)
        {
            var work = new CreateForumWork(m_forumRepository, m_categoryRepository, m_topicRepository, m_messageRepository,
                m_userRepository, m_forumAccessRepository, m_forumSiteUrlHelper, project, bookTypeIds, user, hostUrl);
            var resultId = work.Execute();
            return resultId;
        }

        public void UpdateForum(ProjectDetailContract project, short[] bookTypeIds, UserDetailContract user, string hostUrl)
        {
            new UpdateForumWork(m_forumRepository, m_topicRepository, m_messageRepository,
                m_userRepository, project,  bookTypeIds, user, hostUrl).Execute();
        }
    }
}