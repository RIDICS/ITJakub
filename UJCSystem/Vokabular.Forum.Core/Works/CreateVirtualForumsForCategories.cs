using System.Collections.Generic;
using System.Linq;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Works.Subworks;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class CreateVirtualForumsForCategories : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly ForumAccessSubwork m_forumAccessSubwork;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;
        private readonly IList<int> m_categoryIds;
        private readonly IList<int> m_oldCategoryIds;
        private readonly long m_projectId;

        public CreateVirtualForumsForCategories(ForumRepository forumRepository, ForumAccessSubwork forumAccessSubwork,
            ForumSiteUrlHelper forumSiteUrlHelper, IList<int> categoryIds, IList<int> oldCategoryIds, long projectId) : base(
            forumRepository)
        {
            m_forumRepository = forumRepository;
            m_forumAccessSubwork = forumAccessSubwork;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
            m_categoryIds = categoryIds;
            m_oldCategoryIds = oldCategoryIds;
            m_projectId = projectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            IList<int> newCategories = m_categoryIds.Except(m_oldCategoryIds).ToList();
            IList<int> deletedCategories = m_oldCategoryIds.Except(m_categoryIds).ToList();

            var mainForum = m_forumRepository.GetMainForumByExternalProjectId(m_projectId);

            var forumCategoriesToDelete = m_forumRepository.GetForumsByExternalCategoryIds(deletedCategories);
            var forums = m_forumRepository.GetForumsByExternalProjectId(m_projectId);
            foreach (var forum in forums)
            {
                if (forumCategoriesToDelete.Contains(forum.ParentForum))
                {
                    var forumAccesses = m_forumRepository.GetAllAccessesForForum(forum.ForumID);
                    m_forumRepository.DeleteAll(forumAccesses);
                    m_forumRepository.Delete(forum);
                }
            }

            var forumsToCreate = m_forumRepository.GetForumsByExternalCategoryIds(newCategories);
            foreach (var forum in forumsToCreate)
            {
                var tempForum = new Forum(mainForum.Name, forum.Category, (short) ForumTypeEnum.Forum)
                {
                    ExternalProjectId = m_projectId,
                    ParentForum = forum,
                    RemoteURL = m_forumSiteUrlHelper.GetTopicsUrl(mainForum.ForumID)
                };
                m_forumRepository.Create(tempForum);
                m_forumAccessSubwork.SetAdminAccessToForumForAdminGroup(tempForum);
            }
        }
    }
}