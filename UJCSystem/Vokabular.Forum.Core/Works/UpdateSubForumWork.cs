using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Category = Vokabular.ForumSite.DataEntities.Database.Entities.Category;

namespace Vokabular.ForumSite.Core.Works
{
    class UpdateSubForumWork : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly CategoryContract m_updatedCategory;
        private readonly CategoryContract m_oldCategory;

        public UpdateSubForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, CategoryContract updatedCategory,
            CategoryContract oldCategory) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_updatedCategory = updatedCategory;
            m_oldCategory = oldCategory;
        }

        protected override void ExecuteWorkImplementation()
        {
            foreach (UrlBookTypeEnum bookType in BookTypeHelper.GetBookTypeEnumsWithCategories())
            {
                Category category = m_categoryRepository.GetCategoryByExternalId((short) bookType);

                Forum forum = m_forumRepository.GetForumByExternalIdAndCategory(m_oldCategory.Id, category);
                forum.Name = m_updatedCategory.Description;

                if (m_oldCategory.ParentCategoryId != m_updatedCategory.ParentCategoryId)
                {
                    forum.ParentForum = m_updatedCategory.ParentCategoryId == null
                        ? null
                        : m_forumRepository.GetForumByExternalIdAndCategory((int) m_updatedCategory.ParentCategoryId, category);
                }

                m_forumRepository.Update(forum);
            }
        }
    }
}