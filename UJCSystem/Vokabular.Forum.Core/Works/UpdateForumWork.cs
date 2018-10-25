using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.ForumSite.Core.Helpers;
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
        private readonly short[] m_bookTypeIds;
        private readonly string m_hostUrl;


        public UpdateForumWork(ForumRepository forumRepository, TopicRepository topicRepository, MessageRepository messageRepository,
            UserRepository userRepository, ProjectDetailContract project, short[] bookTypeIds,
            string hostUrl) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_userRepository = userRepository;
            m_project = project;
            m_bookTypeIds = bookTypeIds;
            m_hostUrl = hostUrl;
        }

        protected override void ExecuteWorkImplementation()
        {
            Forum mainForum = m_forumRepository.GetMainForumByExternalProjectId(m_project.Id);

            Topic infoTopic = m_topicRepository.GetFirstTopicInForum(mainForum);
            User user = m_userRepository.GetUserByEmail("info@ridics.cz");
            PostMessageInTopic(infoTopic, user);
            
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

        private void PostMessageInTopic(Topic topic, User user)
        {
            string messageText = $@"Nová publikace: {m_project.Name}
[url={VokabularUrlHelper.GetBookUrl(m_project.Id, m_bookTypeIds.First(), m_hostUrl)}]Odkaz na knihu ve Vokabuláři webovém[/url]";

            Message message = new Message(topic, user, DateTime.UtcNow, messageText);
            m_messageRepository.Create(message);
        }
    }
}