using System;
using NHibernate;

namespace Vokabular.Shared.DataEntities.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        ISession CurrentSession { get; }
        void Commit();
        void Rollback();
    }
}