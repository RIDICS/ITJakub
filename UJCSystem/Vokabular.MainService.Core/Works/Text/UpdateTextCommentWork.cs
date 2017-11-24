using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Text
{
    public class UpdateTextCommentWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_commentId;
        private readonly UpdateTextCommentContract m_data;
        private readonly int m_userId;

        public UpdateTextCommentWork(ResourceRepository resourceRepository, long commentId, UpdateTextCommentContract data, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_commentId = commentId;
            m_data = data;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var comment = m_resourceRepository.FindById<TextComment>(m_commentId);

            OwnershipHelper.CheckItemOwnership(comment.CreatedByUser.Id, m_userId);

            comment.Text = m_data.Text;
            comment.LastEditTime = now;
            comment.EditCount = comment.EditCount + 1 ?? 1;

            m_resourceRepository.Update(comment);
        }
    }
}