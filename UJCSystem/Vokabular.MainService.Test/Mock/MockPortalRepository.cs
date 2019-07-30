using System;
using System.Collections;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace Vokabular.MainService.Test.Mock
{
    public class MockPortalRepository : PortalRepository
    {
        public MockPortalRepository() : base(MockUnitOfWorkProvider.Create())
        {
            CreatedObjects = new List<object>();
            UpdatedObjects = new List<object>();
        }

        public List<object> CreatedObjects { get; }
        public List<object> UpdatedObjects { get; }

        public override object FindById(Type type, object id)
        {
            return Load(type, id);
        }

        public override T FindById<T>(object id)
        {
            return (T)Load(typeof(T), id);
        }

        public override object Load(Type type, object id)
        {
            switch (type.Name)
            {
                case "Project":
                    return new Project
                    {
                        Id = (long)id
                    };
                case "User":
                    return new User
                    {
                        Id = (int)id
                    };
                case "Resource":
                    return new Resource
                    {
                        Id = (long)id
                    };
                case "HeadwordResource":
                    return new HeadwordResource
                    {
                        Id = (long)id
                    };
                default:
                    return null;
            }
        }

        public override T Load<T>(object id)
        {
            return (T)Load(typeof(T), id);
        }

        public override object Create(object instance)
        {
            CreatedObjects.Add(instance);
            return 446L;
        }

        public override IList<object> CreateAll(IEnumerable data)
        {
            throw new NotSupportedException();
        }

        public override void Delete(object instance)
        {
            throw new NotSupportedException();
        }

        public override void Update(object instance)
        {
            UpdatedObjects.Add(instance);
        }

        public override void DeleteAll(Type type)
        {
            throw new NotSupportedException();
        }

        public override void DeleteAll(IEnumerable data)
        {
            throw new NotSupportedException();
        }

        public override void Save(object instance)
        {
            throw new NotSupportedException();
        }

        public override void SaveAll(IEnumerable data)
        {
            throw new NotSupportedException();
        }
    }
}