using System.Net;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Category = Vokabular.ForumSite.DataEntities.Database.Entities.Category;

namespace Vokabular.ForumSite.Core.Works
{
    class DeleteSubForumWork : UnitOfWorkBase
    {
        private readonly ForumRepository m_forumRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly int m_categoryId;
        private readonly ForumAccessRepository m_forumAccessRepository;

        public DeleteSubForumWork(ForumRepository forumRepository, CategoryRepository categoryRepository, ForumAccessRepository forumAccessRepository, int categoryId) : base(forumRepository)
        {
            m_forumRepository = forumRepository;
            m_categoryRepository = categoryRepository;
            m_forumAccessRepository = forumAccessRepository;
            m_categoryId = categoryId;
        }

        protected override void ExecuteWorkImplementation()
        {
            foreach (UrlBookTypeEnum bookType in BookTypeHelper.GetBookTypeEnumsWithCategories())
            {
                Category category = m_categoryRepository.GetCategoryByExternalId((short)bookType);
                Forum forum = m_forumRepository.GetForumByExternalIdAndCategory(m_categoryId, category);

                if (forum == null)
                    throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

                if (forum.Forums.Count > 0)
                    throw new HttpErrorCodeException("Category has some sub-categories", HttpStatusCode.BadRequest);

                m_forumAccessRepository.RemoveAllAccessesFromForum(forum);
                m_forumRepository.Delete(forum);
            }
        }
    }
}