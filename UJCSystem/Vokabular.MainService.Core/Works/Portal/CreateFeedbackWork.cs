using System;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Portal
{
    public class CreateFeedbackWork : UnitOfWorkBase<long>
    {
        public CreateFeedbackWork(PortalRepository portalRepository) : base(portalRepository)
        {
        }

        protected override long ExecuteWorkImplementation()
        {
            throw new NotImplementedException();
        }
    }
}
