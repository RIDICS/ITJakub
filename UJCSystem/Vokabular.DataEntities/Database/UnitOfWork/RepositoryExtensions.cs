using System;
using Vokabular.DataEntities.Database.Daos;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public static class RepositoryExtensions
    {
        public static T InvokeUnitOfWork<T>(this NHibernateDao dao, Func<T> repositoryMethod)
        {
            T result;
            var unitOfWork = dao.UnitOfWork;

            try
            {
                unitOfWork.BeginTransaction();

                result = repositoryMethod.Invoke();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            unitOfWork.Commit();
            return result;
        }
    }
}
