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

        public MetadataResource GetLatestMetadataResource(long projectId, bool includePublisher)
        {
            Resource resourceAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.ResourceType == ResourceTypeEnum.ProjectMetadata && resourceAlias.LatestVersion.Id == x.Id)
                .Fetch(x => x.Resource).Eager;

            if (includePublisher)
            {
                query = query.Fetch(x => x.Publisher).Eager;
            }

            return query.SingleOrDefault();
        }

        public Project GetAdditionalProjectMetadata(long projectId, bool includeAuthors, bool includeResponsibles, bool includeKind, bool includeGenre)
        {
            var session = GetSession();

            if (includeAuthors)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.Authors).Eager
                    .FutureValue();
            }
            if (includeResponsibles)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.ResponsiblePersons).Eager
                    .FutureValue();
            }
            if (includeKind)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.LiteraryKinds).Eager
                    .FutureValue();
            }
            if (includeGenre)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.LiteraryGenres).Eager
                    .FutureValue();
            }

            return session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .FutureValue().Value;
        }
    }
}