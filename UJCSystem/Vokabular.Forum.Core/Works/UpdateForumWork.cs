using System;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class UpdateForumWork : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly TopicRepository m_topicRepository;
        private readonly MessageRepository m_messageRepository;
        private readonly UserRepository m_userRepository;
        private readonly ProjectDetailContract m_project;
        private readonly string m_messageText;
        private readonly string m_defaultAuthorUsername;


        public UpdateForumWork(ForumRepository forumRepository, TopicRepository topicRepository, MessageRepository messageRepository,
            UserRepository userRepository, ProjectDetailContract project, string messageText, string defaultAuthorUsername) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_userRepository = userRepository;
            m_project = project;
            m_messageText = messageText;
            m_defaultAuthorUsername = defaultAuthorUsername;
        }

        protected override void ExecuteWorkImplementation()
        {
            var mainForum = m_forumRepository.GetMainForumByExternalProjectId(m_project.Id);

            var infoTopic = m_topicRepository.GetFirstTopicInForum(mainForum);
            var user = m_userRepository.GetUserByUserName(m_defaultAuthorUsername);
            PostMessageInTopic(infoTopic, user, m_messageText);
            
            if (mainForum.Name != m_project.Name)
            {
                mainForum.Name = m_project.Name;
                m_forumRepository.Update(mainForum);

                var forums = m_forumRepository.GetForumsByExternalProjectId(m_project.Id);
                foreach (var forum in forums)
                {
                    forum.Name = m_project.Name;
                    m_forumRepository.Update(forum);
                }
            }
        }

        private void PostMessageInTopic(Topic topic, User user, string messageText)
        {
            var message = new Message(topic, user, DateTime.UtcNow, messageText);
            m_messageRepository.Create(message);
        }
    }
}