using System.Linq;
using Microsoft.Extensions.Options;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Options;
using Vokabular.ForumSite.Core.Works;
using Vokabular.ForumSite.Core.Works.Subworks;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Managers
{
    public class ForumManager
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly UserRepository m_userRepository;
        private readonly ForumAccessSubwork m_forumAccessSubwork;
        private readonly MessageSubwork m_messageSubwork;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;
        private readonly ForumDefaultMessageGenerator m_forumDefaultMessageGenerator;
        private readonly ForumOption m_forumOptions;

        public ForumManager(ForumRepository forumRepository, CategoryRepository categoryRepository,
            UserRepository userRepository, ForumAccessSubwork forumAccessSubwork, MessageSubwork messageSubwork,
            ForumSiteUrlHelper forumSiteUrlHelper, ForumDefaultMessageGenerator forumDefaultMessageGenerator, IOptions<ForumOption> forumOptions)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_userRepository = userRepository;
            m_forumAccessSubwork = forumAccessSubwork;
            m_messageSubwork = messageSubwork;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
            m_forumDefaultMessageGenerator = forumDefaultMessageGenerator;
            m_forumOptions = forumOptions.Value;
        }

        public int CreateNewForum(ProjectDetailContract project, short[] bookTypeIds)
        {
            var messageText = m_forumDefaultMessageGenerator.GetCreateMessage(project, bookTypeIds.First(), m_forumOptions.WebHubUrl);
            var work = new CreateForumWork(m_categoryRepository, m_userRepository,
                m_forumAccessSubwork, m_messageSubwork, m_forumSiteUrlHelper, project, bookTypeIds, messageText,
                m_forumOptions.DefaultAuthorUsername, m_forumOptions.FirstTopicName);
            var resultId = work.Execute();
            return resultId;
        }

        public void UpdateForum(ProjectDetailContract project, short[] bookTypeIds, string hostUrl)
        {
            var messageText = m_forumDefaultMessageGenerator.GetUpdateMessage(project, bookTypeIds.First(), hostUrl);
            new UpdateForumWork(m_forumRepository, m_messageSubwork,
                m_userRepository, project, messageText, m_forumOptions.DefaultAuthorUsername).Execute();
        }

        public Forum GetForumByExternalId(long projectId)
        {
            return m_forumRepository.InvokeUnitOfWork(x => x.GetMainForumByExternalProjectId(projectId));
        }
    }
}