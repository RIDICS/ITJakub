using System;
using System.Linq;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Works.Subworks;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class CreateForumWork : UnitOfWorkBase<int>
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly TopicRepository m_topicRepository;
        private readonly UserRepository m_userRepository;
        private readonly ForumAccessSubwork m_forumAccessSubwork;
        private readonly MessageSubwork m_messageSubwork;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;
        private readonly ProjectDetailContract m_project;
        private readonly short[] m_bookTypeIds;
        private readonly string m_messageText;
        private readonly string m_username;
        private readonly string m_firstTopicName;
        
        public CreateForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            UserRepository userRepository, ForumAccessSubwork forumAccessSubwork, MessageSubwork messageSubwork,
            ForumSiteUrlHelper forumSiteUrlHelper, ProjectDetailContract project, short[] bookTypeIds, string messageText, string username,
            string firstTopicName) : base(
            forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_topicRepository = topicRepository;
            m_userRepository = userRepository;
            m_forumAccessSubwork = forumAccessSubwork;
            m_messageSubwork = messageSubwork;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
            m_project = project;
            m_bookTypeIds = bookTypeIds;
            m_messageText = messageText;
            m_username = username;
            m_firstTopicName = firstTopicName;
        }

        protected override int ExecuteWorkImplementation()
        {
            var category = m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds.First());

            var forum = new Forum(m_project.Name, category, (short) ForumTypeEnum.Forum) {ExternalProjectId = m_project.Id};
            m_forumRepository.Create(forum);
            m_forumAccessSubwork.SetAdminAccessToForumForAdminGroup(forum);
            m_forumAccessSubwork.SetMemberAccessToForumForRegisteredGroup(forum);

            var user = m_userRepository.GetUserByUserName(m_username);
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
                m_forumAccessSubwork.SetAdminAccessToForumForAdminGroup(tempForum);
                m_forumAccessSubwork.SetMemberAccessToForumForRegisteredGroup(tempForum);
            }
        }

        private void CreateFirstTopicWithMessage(Forum forum, User user, string messageText)
        {
            var firstTopic = new Topic(forum, DateTime.UtcNow, m_firstTopicName,
                (short) TopicTypeEnum.Announcement, user);
            m_topicRepository.Create(firstTopic);

            m_messageSubwork.PostMessageInTopic(firstTopic, user, messageText);

            forum.NumTopics++;
            m_forumRepository.Update(forum);
        }
    }
}