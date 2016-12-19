using System;
using NHibernate;

namespace Vokabular.DataEntities.Database.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ISession CurrentSession { get; }
        void Commit();
        void Rollback();
    }
}