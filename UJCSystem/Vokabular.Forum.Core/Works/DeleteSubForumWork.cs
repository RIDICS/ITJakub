using System.Net;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.Core.Works
{
    public class DeleteSubForumWork : UnitOfWorkBase
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
                var category = m_categoryRepository.GetCategoryByExternalId((short)bookType);
                var forum = m_forumRepository.GetForumByExternalCategoryIdAndCategory(m_categoryId, category);

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