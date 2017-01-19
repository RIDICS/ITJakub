using System;
using NHibernate;

namespace ITJakub.Web.DataEntities.Database.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        ISession CurrentSession { get; }
        void Commit();
        void Rollback();
    }
}