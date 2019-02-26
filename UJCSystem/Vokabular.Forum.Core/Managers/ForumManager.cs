using System.Linq;
using Microsoft.Extensions.Options;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Options;
using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.DataEntities.Database.Entities;
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
        private readonly MessageGenerator m_messageGenerator;
        private readonly ForumOption m_forumOptions;

        public ForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            MessageRepository messageRepository, UserRepository userRepository, ForumAccessRepository forumAccessRepository,
            ForumSiteUrlHelper forumSiteUrlHelper, MessageGenerator messageGenerator, IOptions<ForumOption> forumOptions)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_userRepository = userRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
            m_messageGenerator = messageGenerator;
            m_forumOptions = forumOptions.Value;
        }

        public int CreateNewForum(ProjectDetailContract project, short[] bookTypeIds, string hostUrl)
        {
            var messageText = m_messageGenerator.GetCreateMessage(project, bookTypeIds.First(), hostUrl);
            var work = new CreateForumWork(m_forumRepository, m_categoryRepository, m_topicRepository, m_messageRepository,
                m_userRepository, m_forumAccessRepository, m_forumSiteUrlHelper, project, bookTypeIds, messageText, m_forumOptions.DefaultAuthorUsername, m_forumOptions.FirstTopicName);
            var resultId = work.Execute();
            return resultId;
        }

        public void UpdateForum(ProjectDetailContract project, short[] bookTypeIds, string hostUrl)
        {
            var messageText = m_messageGenerator.GetUpdateMessage(project, bookTypeIds.First(), hostUrl);
            new UpdateForumWork(m_forumRepository, m_topicRepository, m_messageRepository,
                m_userRepository, project, messageText, m_forumOptions.DefaultAuthorUsername).Execute();
        }

        public Forum GetForumByExternalId(long projectId)
        {
            return new GetForumWork(m_forumRepository, projectId).Execute();
        }
    }
}