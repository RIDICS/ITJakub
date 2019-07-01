using Vokabular.ForumSite.Core.Works.Subworks;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class UpdateForumWork : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly MessageSubwork m_messageSubwork;
        private readonly UserRepository m_userRepository;
        private readonly ProjectDetailContract m_project;
        private readonly string m_messageText;
        private readonly string m_defaultAuthorUsername;
        
        public UpdateForumWork(ForumRepository forumRepository, MessageSubwork messageSubwork,
            UserRepository userRepository, ProjectDetailContract project, string messageText, string defaultAuthorUsername) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_messageSubwork = messageSubwork;
            m_userRepository = userRepository;
            m_project = project;
            m_messageText = messageText;
            m_defaultAuthorUsername = defaultAuthorUsername;
        }

        protected override void ExecuteWorkImplementation()
        {
            var mainForum = m_forumRepository.GetMainForumByExternalProjectId(m_project.Id);

            var infoTopic = m_forumRepository.GetFirstTopicInForum(mainForum.ForumID);
            var user = m_userRepository.GetUserByUserName(m_defaultAuthorUsername);
            m_messageSubwork.PostMessageInTopic(infoTopic, user, m_messageText);
            
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
    }
}