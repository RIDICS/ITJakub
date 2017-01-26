using System;
using Vokabular.DataEntities.Database.Daos;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public static class RepositoryExtensions
    {
        public static T InvokeUnitOfWork<T>(this IDao dao, Func<T> repositoryMethod)
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

        public static void InvokeUnitOfWork(this IDao dao, Action repositoryMethod)
        {
            Func<object> functionWrapper = () =>
            {
                repositoryMethod.Invoke();
                return null;
            };
            dao.InvokeUnitOfWork(functionWrapper);
        }
    }
}
