using NHibernate;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ITransaction m_transaction;

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            CurrentSession = sessionFactory.OpenSession();
            CurrentSession.FlushMode = FlushMode.Commit;
            m_transaction = CurrentSession.BeginTransaction();
        }

        public ISession CurrentSession { get; private set; }

        public void Dispose()
        {
            CurrentSession.Close();
            CurrentSession = null;
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
                CurrentSession.Dispose();
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
                CurrentSession.Dispose();
            }
        }
    }
}
