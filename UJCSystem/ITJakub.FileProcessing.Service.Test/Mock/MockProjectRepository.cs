using System;
using System.Collections;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Service.Test.Mock
{
    public class MockProjectRepository : ProjectRepository
    {
        public MockProjectRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
            CreatedObjects = new List<object>();
            UpdatedObjects = new List<object>();
            DeletedObjects = new List<object>();
        }

        public static readonly long GetProjectIdValue = 555;

        public List<ProjectOriginalAuthor> ProjectOriginalAuthors { get; set; }
        public bool CanFindProjectByExternalId { get; set; }
        public List<object> CreatedObjects { get; }
        public List<object> UpdatedObjects { get; }
        public List<object> DeletedObjects { get; }

        public override object FindById(Type type, object id)
        {
            switch (type.Name)
            {
                case "Project":
                    return new Project
                    {
                        Id = (long)id
                    };
                default:
                    return null;
            }
        }

        public override object Load(Type type, object id)
        {
            switch (type.Name)
            {
                case "User":
                    return new User
                    {
                        Id = (int) id
                    };
                case "Project":
                    return new Project
                    {
                        Id = (long) id
                    };
                default:
                    return null;
            }
        }

        public override T FindById<T>(object id)
        {
            return (T) FindById(typeof(T), id);
        }

        public override T Load<T>(object id)
        {
            return (T) Load(typeof(T), id);
        }

        public override object Create(object instance)
        {
            CreatedObjects.Add(instance);

            switch (instance.GetType().Name)
            {
                case "Project":
                    return 555L;
                default:
                    return 555;
            }
        }

        public override IList<object> CreateAll(IEnumerable data)
        {
            throw new NotSupportedException();
        }

        public override void Delete(object instance)
        {
            DeletedObjects.Add(instance);
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

        public override Project GetProjectByExternalId(string externalId, ProjectTypeEnum projectType)
        {
            if (CanFindProjectByExternalId)
            {
                return new Project
                {
                    Id = GetProjectIdValue,
                    ProjectType = projectType,
                    ExternalId = externalId
                };
            }

            return null;
        }

        public override IList<ProjectOriginalAuthor> GetProjectOriginalAuthorList(long projectId, bool includeAuthors = false)
        {
            return ProjectOriginalAuthors;
        }
    }
}
