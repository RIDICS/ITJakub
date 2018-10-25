using System;
using System.Collections.Generic;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    class UpdateForumWork : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly TopicRepository m_topicRepository;
        private readonly MessageRepository m_messageRepository;
        private readonly UserRepository m_userRepository;
        private readonly ProjectDetailContract m_project;
        private readonly string m_messageText;


        public UpdateForumWork(ForumRepository forumRepository, TopicRepository topicRepository, MessageRepository messageRepository,
            UserRepository userRepository, ProjectDetailContract project, string messageText) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_userRepository = userRepository;
            m_project = project;
            m_messageText = messageText;
        }

        protected override void ExecuteWorkImplementation()
        {
            Forum mainForum = m_forumRepository.GetMainForumByExternalProjectId(m_project.Id);

            Topic infoTopic = m_topicRepository.GetFirstTopicInForum(mainForum);
            User user = m_userRepository.GetUserByEmail("info@ridics.cz");
            PostMessageInTopic(infoTopic, user, m_messageText);
            
            if (mainForum.Name != m_project.Name)
            {
                mainForum.Name = m_project.Name;
                m_forumRepository.Update(mainForum);

                IList<Forum> forums = m_forumRepository.GetForumsByExternalProjectId(m_project.Id);
                foreach (Forum forum in forums)
                {
                    forum.Name = m_project.Name;
                    m_forumRepository.Update(forum);
                }
            }
        }

        private void PostMessageInTopic(Topic topic, User user, string messageText)
        {
            Message message = new Message(topic, user, DateTime.UtcNow, messageText);
            m_messageRepository.Create(message);
        }
    }
}