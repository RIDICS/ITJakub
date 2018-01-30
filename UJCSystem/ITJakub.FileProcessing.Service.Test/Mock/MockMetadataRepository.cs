using System;
using System.Collections;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.FileProcessing.Service.Test.Mock
{
    public class MockMetadataRepository : MetadataRepository
    {
        public MockMetadataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            CreatedObjects = new List<object>();
            UpdatedObjects = new List<object>();
            DeletedObjects = new List<object>();
        }

        public List<object> CreatedObjects { get; }
        public List<object> UpdatedObjects { get; }
        public List<object> DeletedObjects { get; }
        public bool CanGetLatestMetadata { get; set; }
        public int LatestMetadataVersion { get; set; }

        public override object FindById(Type type, object id)
        {
            throw new NotSupportedException();
        }

        public override object Load(Type type, object id)
        {
            switch (type.Name)
            {
                case "OriginalAuthor":
                    return new OriginalAuthor()
                    {
                        Id = (int)id
                    };
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
                default:
                    return null;
            }
        }

        public override T FindById<T>(object id)
        {
            return (T)FindById(typeof(T), id);
        }

        public override T Load<T>(object id)
        {
            return (T)Load(typeof(T), id);
        }

        public override object Create(object instance)
        {
            CreatedObjects.Add(instance);
            return 123;
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

        public override MetadataResource GetLatestMetadataResource(long projectId)
        {
            if (CanGetLatestMetadata)
            {
                var metadata = new MetadataResource
                {
                    Id = 14,
                    Resource = new Resource
                    {
                        Project = new Project {Id = projectId},
                        ContentType = ContentTypeEnum.None,
                        ResourceType = ResourceTypeEnum.ProjectMetadata,
                    },
                    VersionNumber = LatestMetadataVersion
                };
                metadata.Resource.LatestVersion = metadata;
                return metadata;
            }

            return null;
        }
    }
}