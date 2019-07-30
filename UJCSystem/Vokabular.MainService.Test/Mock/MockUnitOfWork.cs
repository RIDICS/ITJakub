using NHibernate;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Test.Mock
{
    public class MockUnitOfWork : IUnitOfWork
    {
        public bool IsTransactionStarted { get; set; }
        public bool IsDisposed { get; set; }
        public bool IsCommited { get; set; }
        public bool IsRollback { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void BeginTransaction()
        {
            IsTransactionStarted = true;
        }

        public ISession CurrentSession
        {
            get { return null; }
        }

        public void Commit()
        {
            IsTransactionStarted = false;
            IsCommited = true;
        }

        public void Rollback()
        {
            IsTransactionStarted = false;
            IsRollback = true;
        }
    }
}