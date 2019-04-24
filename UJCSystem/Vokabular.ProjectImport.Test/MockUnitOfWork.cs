using NHibernate;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.ProjectImport.Test
{
    public class MockUnitOfWork : IUnitOfWork
    {
        private readonly ISessionFactory m_sessionFactory;
        private ITransaction m_transaction;

        public MockUnitOfWork(ISessionFactory sessionFactory)
        {
            m_sessionFactory = sessionFactory;
        }

        public void BeginTransaction()
        {
            if (m_transaction != null && m_transaction.IsActive)
            {
                return;
            }

            CurrentSession = m_sessionFactory.OpenSession();
            CurrentSession.FlushMode = FlushMode.Commit;
            m_transaction = CurrentSession.BeginTransaction();
        }

        public ISession CurrentSession { get; private set; }

       public virtual void Dispose()
        {
        }

        public void Commit()
        {
            try
            {
                if (m_transaction.IsActive)
                {
                    m_transaction.Commit();
                }
            }
            catch
            {
                if (m_transaction.IsActive)
                {
                    m_transaction.Rollback();
                }
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public void Rollback()
        {
            try
            {
                if (m_transaction.IsActive)
                {
                    m_transaction.Rollback();
                }
            }
            finally
            {
                Dispose();
            }
        }
    }
}
