using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class MetadataRepository : NHibernateDao
    {
        public MetadataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<Publisher> GetPublisherList()
        {
            return GetSession().QueryOver<Publisher>()
                .OrderBy(x => x.Text).Asc
                .List();
        }

        public IList<LiteraryKind> GetLiteraryKindList()
        {
            return GetSession().QueryOver<LiteraryKind>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public IList<LiteraryGenre> GetLiteraryGenreList()
        {
            return GetSession().QueryOver<LiteraryGenre>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public MetadataResource GetLatestMetadataResource(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.ResourceType == ResourceTypeEnum.ProjectMetadata)
                .Fetch(x => x.Resource).Eager
                .SingleOrDefault();
        }
    }
}