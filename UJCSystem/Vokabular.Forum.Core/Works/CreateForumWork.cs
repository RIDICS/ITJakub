using System;
using System.Linq;
using Vokabular.ForumSite.Core.Helpers;
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
        private readonly UserDetailContract m_user;


        public CreateForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, TopicRepository topicRepository,
            MessageRepository messageRepository, UserRepository userRepository, ForumAccessRepository forumAccessRepository,
            ForumSiteUrlHelper forumSiteUrlHelper, ProjectDetailContract project, short[] bookTypeIds, UserDetailContract user) : base(
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
            m_user = user;
        }

        protected override long ExecuteWorkImplementation()
        {
            Category category = m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds.First());

            Forum forum = new Forum(m_project.Name, category, (short) ForumTypeEnum.Forum) {ExternalProjectId = m_project.Id};
            m_forumRepository.Create(forum);
            m_forumAccessRepository.SetAdminAccessToForumForAdminGroup(forum); //TODO set access to forum
            CreateVirtualForumsForOtherBookTypes(forum);

            //User user = m_userRepository.GetUserByEmail(m_user.Email); //TODO connect with Vokabular
            User user = m_userRepository.GetUserByEmail("tomas.hrabacek@scalesoft.cz");

            Topic firstTopic = CreateFirstTopic(forum, user);
            Message firstMessage = CreateFirstMessageInTopic(firstTopic, user);

            firstTopic.LastMessage = firstMessage;
            firstTopic.LastUser = firstMessage.User;
            firstTopic.LastPosted = firstMessage.Posted;
            firstTopic.LastMessageFlags = firstMessage.Flags;
            firstTopic.NumPosts++;

            m_topicRepository.Update(firstTopic);

            forum.LastPosted = firstTopic.Posted;
            forum.LastTopic = firstTopic;
            forum.LastUser = firstTopic.User;
            forum.NumTopics++;
            forum.NumPosts++;
            forum.LastUserDisplayName = firstMessage.UserDisplayName;
            forum.LastMessage = firstMessage;

            m_forumRepository.Update(forum);

            return forum.ForumID;
        }

        private void CreateVirtualForumsForOtherBookTypes(Forum forum)
        {
            for (int i = 1; i < m_bookTypeIds.Length; i++)
            {
                Forum tempForum = new Forum(m_project.Name, m_categoryRepository.GetCategoryByExternalId(m_bookTypeIds[i]),
                    (short) ForumTypeEnum.Forum) {ExternalProjectId = m_project.Id};
                tempForum.RemoteURL = m_forumSiteUrlHelper.GetTopicsUrl(forum.ForumID);
                m_forumRepository.Create(tempForum);
                m_forumAccessRepository.SetAdminAccessToForumForAdminGroup(tempForum); //TODO set access to forum
            }
        }

        private Topic CreateFirstTopic(Forum forum, User user)
        {
            Topic firstTopic = new Topic(forum, DateTime.UtcNow, FirstTopicName,
                (short) TopicTypeEnum.Announcement, user);
            m_topicRepository.Create(firstTopic);
            return firstTopic;
        }

        private Message CreateFirstMessageInTopic(Topic topic, User user)
        {
            string authors = "";
            if (m_project.Authors != null)
            {
                foreach (var author in m_project.Authors)
                {
                    authors += author.FirstName + " " + author.LastName + Environment.NewLine;
                }
            }

            string messageText = $@"{m_project.Name}
[url={VokabularUrlHelper.GetBookUrl(m_project.Id, m_bookTypeIds.First())}]Odkaz na knihu ve Vokabuláři webovém[/url]
{(m_project.Authors == null ? "Autor: <Nezadáno>" : (m_project.Authors.Count == 1 ? "Autor:" : "Autoři:"))} {authors}
Počet stran: {(m_project.PageCount == null ? "<Nezadáno>" : m_project.PageCount.ToString())}";

            Message firstMessage = new Message(topic, user, DateTime.UtcNow, messageText);
            m_messageRepository.Create(firstMessage);
            return firstMessage;
        }
    }
}