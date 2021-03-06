﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.Search
{
    public class SearchCriteriaQueryCreator : ISearchCriteriaCreator
    {
        private const string FromClause = "from MetadataResource metadata inner join metadata.Resource resource inner join resource.Project project inner join project.LatestPublishedSnapshot snapshot left outer join snapshot.BookVersion bookVersion";
        // inner join to LatestPublishedSnapshots filters result only to published Projects
        private const string ResultFromClause = "from MetadataResource metadata1 inner join metadata1.Resource resource1";// left outer join resource1.Project project1 inner join project1.LatestPublishedSnapshot snapshot1";

        private readonly List<SearchCriteriaQuery> m_conjunctionQuery;
        private readonly Dictionary<string, object> m_metadataParameters;
        private readonly IList<ProjectTypeEnum> m_projectTypes;

        public SearchCriteriaQueryCreator(List<SearchCriteriaQuery> conjunctionQuery,
            Dictionary<string, object> metadataParameters,
            ProjectTypeEnum projectType) : this(conjunctionQuery, metadataParameters, new List<ProjectTypeEnum> { projectType })
        {
        }

        public SearchCriteriaQueryCreator(List<SearchCriteriaQuery> conjunctionQuery,
            Dictionary<string, object> metadataParameters,
            IList<ProjectTypeEnum> projectTypes)
        {
            m_conjunctionQuery = conjunctionQuery;
            m_metadataParameters = metadataParameters;
            m_projectTypes = projectTypes;
        }

        public SortTypeEnumContract? Sort { get; set; }
        public SortDirectionEnumContract? SortDirection { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }

        public int GetStart()
        {
            return Start;
        }

        public int GetCount()
        {
            return Count;
        }

        public Dictionary<string, object> Parameters => m_metadataParameters;

        public string GetQueryString()
        {
            var joinAndWhereClause = CreateJoinAndWhereClause(m_conjunctionQuery, m_projectTypes);
            var orderByClause = CreateOrderByClause("metadata1");

            var queryString = $"select metadata1 {ResultFromClause} where resource1.LatestVersion.Id = metadata1.Id and resource1.Project.Id in (select distinct project.Id {FromClause} {joinAndWhereClause}) {orderByClause}";

            return queryString;
        }

        public string GetProjectIdentificationListQueryString()
        {
            var joinAndWhereClause = CreateJoinAndWhereClause(m_conjunctionQuery, m_projectTypes);
            
            var queryString = $"select distinct project.Id as ProjectId, project.ExternalId as ProjectExternalId, snapshot.Id as SnapshotId, bookVersion.ExternalId as BookVersionExternalId, project.ProjectType as ProjectType {FromClause} {joinAndWhereClause}";

            return queryString;
        }

        public string GetQueryStringForCount()
        {
            var joinAndWhereClause = CreateJoinAndWhereClause(m_conjunctionQuery, m_projectTypes);

            var queryString = $"select count(distinct metadata.Id) {FromClause} {joinAndWhereClause}";

            return queryString;
        }

        public bool HasHeadwordRestrictions()
        {
            return m_conjunctionQuery.Any(x => x.CriteriaKey == CriteriaKey.Headword);
        }

        public ICriterion GetHeadwordRestrictions()
        {
            var conjunction = new Conjunction();
            foreach (var searchCriteriaQuery in m_conjunctionQuery.Where(x => x.CriteriaKey == CriteriaKey.Headword))
            {
                var disjunction = (Disjunction) searchCriteriaQuery.Restriction;
                conjunction.Add(disjunction);
            }

            return conjunction;
        }

        private string CreateJoinAndWhereClause(List<SearchCriteriaQuery> conjunctionQuery, IList<ProjectTypeEnum> projectType)
        {
            var joinBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();

            var projectTypeValues = string.Join(",", projectType.Cast<short>());
            whereBuilder.Append($" where metadata.Id = resource.LatestVersion.Id and project.ProjectType in ({projectTypeValues}) and project.IsRemoved = 0");

            foreach (var criteriaQuery in conjunctionQuery.Where(x => x.CriteriaKey != CriteriaKey.Headword))
            {
                if (!string.IsNullOrEmpty(criteriaQuery.Join))
                    joinBuilder.Append(' ').Append(criteriaQuery.Join);

                whereBuilder.Append(" and");
                whereBuilder.Append(" (").Append(criteriaQuery.Where).Append(')');
            }

            return string.Format("{0}{1}", joinBuilder, whereBuilder);
        }

        private string CreateOrderByClause(string metadataAlias)
        {
            if (Sort == null)
                return string.Empty;

            switch (Sort.Value)
            {
                case SortTypeEnumContract.Author:
                    return $" order by {metadataAlias}.AuthorsLabel {GetOrderByDirection()}";
                case SortTypeEnumContract.Title:
                    return $" order by {metadataAlias}.Title {GetOrderByDirection()}";
                case SortTypeEnumContract.Dating:
                    return SortDirection == SortDirectionEnumContract.Desc
                        ? $" order by {metadataAlias}.NotAfter desc"
                        : $" order by {metadataAlias}.NotBefore asc";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetOrderByDirection()
        {
            if (SortDirection == null)
                return string.Empty;

            switch (SortDirection.Value)
            {
                case SortDirectionEnumContract.Asc:
                    return "asc";
                case SortDirectionEnumContract.Desc:
                    return "desc";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
