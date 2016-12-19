using System;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public abstract class UnitOfWorkJobBase
    {
        private readonly IUnitOfWork m_unitOfWork;

        protected UnitOfWorkJobBase(IUnitOfWork unitOfWork)
        {
            m_unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            try
            {
                ExecuteImplementation();
            }
            catch (Exception)
            {
                m_unitOfWork.Rollback();
                throw;
            }

            m_unitOfWork.Commit();
        }

        protected abstract void ExecuteImplementation();
    }
}
