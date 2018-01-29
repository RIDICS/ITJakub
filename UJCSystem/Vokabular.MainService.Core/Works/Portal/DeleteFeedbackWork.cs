using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Portal
{
    public class DeleteFeedbackWork : UnitOfWorkBase
    {
        private readonly PortalRepository m_portalRepository;
        private readonly long m_feedbackId;

        public DeleteFeedbackWork(PortalRepository portalRepository, long feedbackId) : base(portalRepository)
        {
            m_portalRepository = portalRepository;
            m_feedbackId = feedbackId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var feedback = m_portalRepository.Load<Feedback>(m_feedbackId);
            m_portalRepository.Delete(feedback);
        }
    }
}