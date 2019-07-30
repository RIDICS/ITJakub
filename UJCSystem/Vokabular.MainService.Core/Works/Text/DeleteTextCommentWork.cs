using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Text
{
    public class DeleteTextCommentWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_commentId;

        public DeleteTextCommentWork(ResourceRepository resourceRepository, long commentId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_commentId = commentId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var dbComment = m_resourceRepository.FindById<TextComment>(m_commentId);

            DeleteCommentsHierarchy(dbComment);
        }

        private void DeleteCommentsHierarchy(TextComment dbComment)
        {
            foreach (var dbSubcomment in dbComment.TextComments)
            {
                DeleteCommentsHierarchy(dbSubcomment);
            }

            m_resourceRepository.Delete(dbComment);
        }
    }
}