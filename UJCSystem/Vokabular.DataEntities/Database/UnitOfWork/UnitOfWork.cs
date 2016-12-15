using System;
using NHibernate;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ISession CurrentSession { get; }
        void Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISessionFactory m_sessionFactory;
        private readonly ITransaction m_transaction;

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            m_sessionFactory = sessionFactory;
            CurrentSession = m_sessionFactory.OpenSession();
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
            m_transaction.Commit();
        }
    }
}
