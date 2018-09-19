using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Category = Vokabular.ForumSite.DataEntities.Database.Entities.Category;

namespace Vokabular.ForumSite.Core.Works
{
    class CreateForumWork : UnitOfWorkBase<long>
    {
        private const string m_TopicDescription = "Základní informace";  //TODO better description
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly TopicRepository m_topicRepository;
        private readonly MessageRepository m_messageRepository;
        private readonly UserRepository m_userRepository;
        private readonly ProjectDetailContract m_project;
        private readonly short[] m_bookTypeIds;
        private readonly UserDetailContract m_user;

        public CreateForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            MessageRepository messageRepository, UserRepository userRepository, ProjectDetailContract project,
            short[] bookTypeIds, UserDetailContract user) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_project = project;
            m_bookTypeIds = bookTypeIds;
            m_user = user;
            m_userRepository = userRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;

            Category category = m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds.First());

            Forum forum = new Forum(m_project.Name, category, (short)ForumTypeEnum.Forum);
            m_forumRepository.Create(forum);
            //TODO create forum access

            CreateVirtualForumsForOtherBookTypes();

            //User user = m_userRepository.GetUserByEmail(m_user.Email); //TODO connect with Vokabular
            User user = m_userRepository.GetUserByEmail("tomas.hrabacek@scalesoft.cz");

            Topic firstTopic = CreateFirstTopic(forum, user);
            CreateFirstMessageInTopic(firstTopic);
           
            return forum.ForumID;
        }

        private void CreateVirtualForumsForOtherBookTypes()
        {
            for (int i = 1; i < m_bookTypeIds.Length; i++)
            {
                Forum tempForum = new Forum(m_project.Name, m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds[i]),
                    (short)ForumTypeEnum.Forum);
                //TODO create and set RemoteURL
                m_forumRepository.Create(tempForum);
                //TODO create forum access
            }
        }

        private Topic CreateFirstTopic(Forum forum, User user)
        {
            Topic firstTopic = new Topic(forum, DateTime.UtcNow, m_TopicDescription, (short)TopicTypeEnum.Announcement, user);
            m_topicRepository.Create(firstTopic);
            return firstTopic;
            //TODO move to worker/manager?
            //TODO set lastTopic to Forum
        }

        private void CreateFirstMessageInTopic(Topic topic)
        {
            //TODO create first message to topic
        }
    }
}