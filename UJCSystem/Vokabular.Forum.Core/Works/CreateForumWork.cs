using System;
using System.Linq;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class CreateForumWork : UnitOfWorkBase<int>
    {
        private const string FirstTopicName = "Základní informace";
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly TopicRepository m_topicRepository;
        private readonly MessageRepository m_messageRepository;
        private readonly UserRepository m_userRepository;
        private readonly ForumAccessRepository m_forumAccessRepository;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;
        private readonly ProjectDetailContract m_project;
        private readonly short[] m_bookTypeIds;
        private readonly string m_messageText;


        public CreateForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            MessageRepository messageRepository, UserRepository userRepository, ForumAccessRepository forumAccessRepository,
            ForumSiteUrlHelper forumSiteUrlHelper, ProjectDetailContract project, short[] bookTypeIds, string messageText) : base(
            forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_topicRepository = topicRepository;
            m_messageRepository = messageRepository;
            m_userRepository = userRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
            m_project = project;
            m_bookTypeIds = bookTypeIds;
            m_messageText = messageText;
        }

        protected override int ExecuteWorkImplementation()
        {
            var category = m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds.First());
            
            var forum = new Forum(m_project.Name, category, (short) ForumTypeEnum.Forum) {ExternalProjectId = m_project.Id};
            m_forumRepository.Create(forum);
            m_forumAccessRepository.SetAdminAccessToForumForAdminGroup(forum);
            m_forumAccessRepository.SetMemberAccessToForumForRegisteredGroup(forum);

            var user = m_userRepository.GetUserByEmail("info@ridics.cz");
            CreateFirstTopicWithMessage(forum, user, m_messageText);

            CreateVirtualForumsForOtherBookTypes(forum);
            
            return forum.ForumID;
        }
        
        private void CreateVirtualForumsForOtherBookTypes(Forum forum)
        {
            for (var i = 1; i < m_bookTypeIds.Length; i++)
            {
                var tempForum = new Forum(m_project.Name, m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds[i]),
                    (short) ForumTypeEnum.Forum)
                {
                    ExternalProjectId = m_project.Id,
                    RemoteURL = m_forumSiteUrlHelper.GetTopicsUrl(forum.ForumID)
                };
                m_forumRepository.Create(tempForum);
                m_forumAccessRepository.SetAdminAccessToForumForAdminGroup(tempForum);
                m_forumAccessRepository.SetMemberAccessToForumForRegisteredGroup(tempForum);
            }
        }

        private void CreateFirstTopicWithMessage(Forum forum, User user, string messageText)
        {
            var firstTopic = new Topic(forum, DateTime.UtcNow, FirstTopicName,
                (short) TopicTypeEnum.Announcement, user);
            m_topicRepository.Create(firstTopic);

            PostMessageInTopic(firstTopic, user, messageText);

            forum.NumTopics++;
            m_forumRepository.Update(forum);
        }

        private void PostMessageInTopic(Topic topic, User user, string messageText)
        {
            var message = new Message(topic, user, DateTime.UtcNow, messageText);
            m_messageRepository.Create(message);
        }
    }
}