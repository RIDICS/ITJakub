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
}
