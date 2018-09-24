using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    class CreateVirtualForumsForCategories : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly ForumAccessRepository m_forumAccessRepository;
        private readonly IList<int> m_categoryIds;
        private readonly IList<int> m_oldCategoryIds;
        private readonly long m_projectId;

        public CreateVirtualForumsForCategories(ForumRepository forumRepository, ForumAccessRepository forumAccessRepository,
            IList<int> categoryIds, IList<int> oldCategoryIds, long projectId) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_categoryIds = categoryIds;
            m_oldCategoryIds = oldCategoryIds;
            m_projectId = projectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            IList<int> newCategories = m_categoryIds.Except(m_oldCategoryIds).ToList();
            IList<int> deletedCategories = m_oldCategoryIds.Except(m_categoryIds).ToList();

            var mainForum = m_forumRepository.GetMainForumByExternalProjectId(m_projectId);

            IList<Forum> forumCategoriesToDelete = m_forumRepository.GetForumsByExternalCategoryIds((ICollection) deletedCategories);
            IList<Forum> forums = m_forumRepository.GetForumsByExternalProjectId(m_projectId);
            foreach (var forum in forums)
            {
                if (forumCategoriesToDelete.Contains(forum.ParentForum))
                {
                    m_forumAccessRepository.RemoveAllAccessesFromForum(forum);
                    m_forumRepository.Delete(forum);
                }
            }
            
            IList<Forum> forumsToCreate = m_forumRepository.GetForumsByExternalCategoryIds((ICollection) newCategories);
            foreach (var forum in forumsToCreate)
            {
                Forum tempForum = new Forum(mainForum.Name, forum.Category, (short) ForumTypeEnum.Forum)
                {
                    ExternalProjectId = m_projectId,
                    ParentForum = forum,
                    RemoteURL = ForumSiteUrlHelper.GetTopicsUrl(mainForum.ForumID, mainForum.Name)
                };
                m_forumRepository.Create(tempForum);
                m_forumAccessRepository.SetAdminAccessToForumForAdminGroup(tempForum);
            }
        }
    }
}