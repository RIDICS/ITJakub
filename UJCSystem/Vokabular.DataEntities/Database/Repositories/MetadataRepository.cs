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

        public virtual IList<LiteraryKind> GetLiteraryKindList()
        {
            return GetSession().QueryOver<LiteraryKind>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual IList<LiteraryGenre> GetLiteraryGenreList()
        {
            return GetSession().QueryOver<LiteraryGenre>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual MetadataResource GetLatestMetadataResource(long projectId)
        {
            Resource resourceAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.ResourceType == ResourceTypeEnum.ProjectMetadata && resourceAlias.LatestVersion.Id == x.Id)
                .Fetch(x => x.Resource).Eager;

            return query.SingleOrDefault();
        }

        public virtual Project GetAdditionalProjectMetadata(long projectId, bool includeAuthors, bool includeResponsibles, bool includeKind, bool includeGenre)
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

        public virtual Project GetProjectWithKeywords(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.Keywords).Eager
                .SingleOrDefault();
        }

        public virtual IList<ProjectOriginalAuthor> GetProjectOriginalAuthorList(long projectId, bool includeAuthors = false)
        {
            var query = GetSession().QueryOver<ProjectOriginalAuthor>()
                .Where(x => x.Project.Id == projectId);

            if (includeAuthors)
            {
                query.Fetch(x => x.OriginalAuthor);
            }

            return query.List();
        }

        public virtual OriginalAuthor GetAuthorByName(string firstName, string lastName)
        {
            return GetSession().QueryOver<OriginalAuthor>()
                .Where(x => x.FirstName == firstName && x.LastName == lastName)
                .SingleOrDefault();
        }
        
        public virtual Keyword GetKeywordByName(string name)
        {
            return GetSession().QueryOver<Keyword>()
                .Where(x => x.Text == name)
                .SingleOrDefault();
        }

        public virtual IList<ProjectResponsiblePerson> GetProjectResponsibleList(long projectId)
        {
            return GetSession().QueryOver<ProjectResponsiblePerson>()
                .Where(x => x.Project.Id == projectId)
                .Fetch(x => x.ResponsiblePerson).Eager
                .Fetch(x => x.ResponsibleType).Eager
                .List();
        }

        public virtual ResponsiblePerson GetResponsiblePersonByName(string firstName, string lastName)
        {
            return GetSession().QueryOver<ResponsiblePerson>()
                .Where(x => x.FirstName == firstName && x.LastName == lastName)
                .SingleOrDefault();
        }

        public virtual ResponsibleType GetResponsibleTypeByName(string text)
        {
            return GetSession().QueryOver<ResponsibleType>()
                .Where(x => x.Text == text)
                .SingleOrDefault();
        }
    }
}