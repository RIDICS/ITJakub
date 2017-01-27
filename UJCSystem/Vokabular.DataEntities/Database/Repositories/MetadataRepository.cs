using System.Collections.Generic;
using NHibernate.SqlCommand;
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
                ProjectOriginalAuthor projectAuthorAlias = null;
                OriginalAuthor authorAlias = null;

                session.QueryOver<Project>()
                    .JoinAlias(x => x.Authors, () => projectAuthorAlias, JoinType.LeftOuterJoin)
                    .JoinAlias(() => projectAuthorAlias.OriginalAuthor, () => authorAlias, JoinType.LeftOuterJoin)
                    .Where(x => x.Id == projectId)
                    .OrderBy(() => projectAuthorAlias.Sequence).Asc
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

        public IList<ProjectOriginalAuthor> GetProjectOriginalAuthorList(long projectId, bool includeAuthors = false)
        {
            var query = GetSession().QueryOver<ProjectOriginalAuthor>()
                .Where(x => x.Project.Id == projectId);

            if (includeAuthors)
            {
                query.Fetch(x => x.OriginalAuthor);
            }

            return query.List();
        }

        public OriginalAuthor GetAuthorByName(string firstName, string lastName)
        {
            return GetSession().QueryOver<OriginalAuthor>()
                .Where(x => x.FirstName == firstName && x.LastName == lastName)
                .SingleOrDefault();
        }

        public Publisher GetPublisher(string publisherText, string email)
        {
            return GetSession().QueryOver<Publisher>()
                .Where(x => x.Text == publisherText && x.Email == email)
                .SingleOrDefault();
        }
    }
}