using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class UpdateSubForumWork : UnitOfWorkBase
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
            foreach (var bookType in BookTypeHelper.GetBookTypeEnumsWithCategories())
            {
                var category = m_categoryRepository.GetCategoryByExternalId((short) bookType);

                var forum = m_forumRepository.GetForumByExternalCategoryIdAndCategory(m_oldCategory.Id, category.CategoryID);
                forum.Name = m_updatedCategory.Description;

                if (m_oldCategory.ParentCategoryId != m_updatedCategory.ParentCategoryId)
                {
                    forum.ParentForum = m_updatedCategory.ParentCategoryId == null
                        ? null
                        : m_forumRepository.GetForumByExternalCategoryIdAndCategory((int) m_updatedCategory.ParentCategoryId, category.CategoryID);
                }

                m_forumRepository.Update(forum);
            }
        }
    }
}