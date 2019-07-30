using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Works.Subworks;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class CreateSubForumWork : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ForumAccessSubwork m_forumAccessSubwork;
        private readonly CategoryContract m_category;

        public CreateSubForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository,
            ForumAccessSubwork forumAccessSubwork, CategoryContract category) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessSubwork = forumAccessSubwork;
            m_category = category;
        }

        protected override void ExecuteWorkImplementation()
        {
            foreach (var bookType in BookTypeHelper.GetBookTypeEnumsWithCategories())
            {
                var category = m_categoryRepository.GetCategoryByExternalId((short) bookType);

                Forum parentForum = null;
                if (m_category.ParentCategoryId != null)
                {
                    parentForum = m_forumRepository.GetForumByExternalCategoryIdAndCategory((int) m_category.ParentCategoryId, category.CategoryID);
                }

                var forum = new Forum(m_category.Description, category, (short) ForumTypeEnum.SubCategory)
                {
                    ExternalId = m_category.Id,
                    ParentForum = parentForum
                };
                m_forumRepository.Create(forum);
                m_forumAccessSubwork.SetAdminAccessToForumForAdminGroup(forum);
            }
        }
    }
}