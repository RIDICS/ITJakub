using System;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public abstract class UnitOfWorkBase
    {
        private readonly IUnitOfWork m_unitOfWork;

        protected UnitOfWorkBase(IUnitOfWork unitOfWork)
        {
            m_unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            try
            {
                m_unitOfWork.BeginTransaction();

                ExecuteWorkImplementation();
            }
            catch (Exception)
            {
                m_unitOfWork.Rollback();
                throw;
            }

            m_unitOfWork.Commit();
        }

        protected abstract void ExecuteWorkImplementation();
    }

    public abstract class UnitOfWorkBase<T>
    {
        private readonly IUnitOfWork m_unitOfWork;

        protected UnitOfWorkBase(IUnitOfWork unitOfWork)
        {
            m_unitOfWork = unitOfWork;
        }

        public T Execute()
        {
            T result;
            try
            {
                m_unitOfWork.BeginTransaction();

                result = ExecuteWorkImplementation();
            }
            catch (Exception)
            {
                m_unitOfWork.Rollback();
                throw;
            }

            m_unitOfWork.Commit();
            return result;
        }

        protected abstract T ExecuteWorkImplementation();
    }
}
