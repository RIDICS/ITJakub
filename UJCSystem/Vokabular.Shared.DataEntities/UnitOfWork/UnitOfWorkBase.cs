using System;
using Vokabular.Shared.DataEntities.Daos;

namespace Vokabular.Shared.DataEntities.UnitOfWork
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
            var unitOfWork = m_dao.UnitOfWork;
            try
            {
                unitOfWork.BeginTransaction();

                ExecuteWorkImplementation();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            unitOfWork.Commit();
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
            T result;
            var unitOfWork = m_dao.UnitOfWork;
            try
            {
                unitOfWork.BeginTransaction();

                result = ExecuteWorkImplementation();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            unitOfWork.Commit();
            return result;
        }

        protected abstract T ExecuteWorkImplementation();
    }
}
