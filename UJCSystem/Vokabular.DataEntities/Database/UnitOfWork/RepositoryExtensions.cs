using System;
using Vokabular.DataEntities.Database.Daos;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public static class RepositoryExtensions
    {
        public static TResult InvokeUnitOfWork<TResult, TDao>(this TDao dao, Func<TDao, TResult> repositoryMethod) where TDao : IDao
        {
            TResult result;
            var unitOfWork = dao.UnitOfWork;

            try
            {
                unitOfWork.BeginTransaction();

                result = repositoryMethod.Invoke(dao);
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            unitOfWork.Commit();
            return result;
        }

        public static void InvokeUnitOfWork<TDao>(this TDao dao, Action<TDao> repositoryMethod) where TDao : IDao
        {
            Func<TDao, object> functionWrapper = (x) =>
            {
                repositoryMethod.Invoke(dao);
                return null;
            };
            dao.InvokeUnitOfWork(functionWrapper);
        }
    }
}
