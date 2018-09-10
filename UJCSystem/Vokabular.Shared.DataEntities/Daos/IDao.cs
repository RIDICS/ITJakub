using System;
using System.Collections;
using System.Collections.Generic;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.Shared.DataEntities.Daos
{
    public interface IDao
    {
        IUnitOfWork UnitOfWork { get; }

        object Create(object instance);
        IList<object> CreateAll(IEnumerable data);
        void Delete(object instance);
        void DeleteAll(Type type);
        void DeleteAll(IEnumerable data);
        object FindById(Type type, object id);
        T FindById<T>(object id);
        object Load(Type type, object id);
        T Load<T>(object id);
        void Save(object instance);
        void SaveAll(IEnumerable data);
        void Update(object instance);
    }
}