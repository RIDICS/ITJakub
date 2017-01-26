using Vokabular.DataEntities.Database.Daos;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public abstract class UnitOfWorkBase
    {
        private readonly IDao m_dao;

        protected UnitOfWorkBase(IDao dao)
        {
            m_dao = dao;
        }

        public void Execute()
        {
            m_dao.InvokeUnitOfWork(ExecuteWorkImplementation);
        }

        protected abstract void ExecuteWorkImplementation();
    }

    public abstract class UnitOfWorkBase<T>
    {
        private readonly IDao m_dao;

        protected UnitOfWorkBase(IDao dao)
        {
            m_dao = dao;
        }

        public T Execute()
        {
            return m_dao.InvokeUnitOfWork(ExecuteWorkImplementation);
        }

        protected abstract T ExecuteWorkImplementation();
    }
}
