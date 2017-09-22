using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Text
{
    public class CreateNewTextCommentWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_textId;
        private readonly CreateTextCommentContract m_newComment;
        private readonly int m_userId;

        public CreateNewTextCommentWork(ResourceRepository resourceRepository, long textId, CreateTextCommentContract newComment, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_textId = textId;
            m_newComment = newComment;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            TextComment parentTextComment = null;
            if (m_newComment.ParentCommentId != null)
            {
                parentTextComment = m_resourceRepository.FindById<TextComment>(m_newComment.ParentCommentId);
                if (parentTextComment.ParentComment != null)
                {
                    throw new InvalidOperationException("Only comments to second level are allowed");
                }
                m_newComment.TextReferenceId = parentTextComment.TextReferenceId; // Subcomments must have the same TextReferenceId as parent
            }

            var user = m_resourceRepository.Load<User>(m_userId);
            var resourceText = m_resourceRepository.Load<Resource>(m_textId);
            var newComment = new TextComment
            {
                CreateTime = now,
                CreatedByUser = user,
                ParentComment = parentTextComment,
                ResourceText = resourceText,
                Text = m_newComment.Text,
                TextReferenceId = m_newComment.TextReferenceId
            };
            var resultId = (long) m_resourceRepository.Create(newComment);

            return resultId;
        }
    }
}
