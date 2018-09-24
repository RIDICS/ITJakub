using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Enums;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Category = Vokabular.ForumSite.DataEntities.Database.Entities.Category;

namespace Vokabular.ForumSite.Core.Works
{
    class CreateSubForumWork : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ForumAccessRepository m_forumAccessRepository;
        private readonly CategoryContract m_category;

        public CreateSubForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository,
            ForumAccessRepository forumAccessRepository, CategoryContract category) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_category = category;
        }

        protected override void ExecuteWorkImplementation()
        {
            foreach (UrlBookTypeEnum bookType in BookTypeHelper.GetBookTypeEnumsWithCategories())
            {
                Category category = m_categoryRepository.GetCategoryByExternalId((short) bookType);

                Forum parentForum = null;
                if (m_category.ParentCategoryId != null)
                {
                    parentForum = m_forumRepository.GetForumByExternalIdAndCategory((int) m_category.ParentCategoryId, category);
                }

                Forum forum = new Forum(m_category.Description, category, (short) ForumTypeEnum.SubCategory)
                {
                    ExternalId = m_category.Id,
                    ParentForum = parentForum
                };
                m_forumRepository.Create(forum);
                m_forumAccessRepository.SetAdminAccessToForumForAdminGroup(forum);
            }
        }
    }
}