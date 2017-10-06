using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.Search
{
    public class SearchCriteriaQueryCreator
    {
        private const int MaxCount = 100;
        private const int DefaultCount = 10;
        private const int DefaultStart = 0;
        private const string FromClause = "from MetadataResource metadata inner join metadata.Resource resource inner join resource.Project project inner join project.LatestPublishedSnapshot snapshot";
        // inner join to LatestPublishedSnapshots filters result only to published Projects
        private const string ResultFromClause = "from MetadataResource metadata1 inner join metadata1.Resource resource1";// left outer join resource1.Project project1 inner join project1.LatestPublishedSnapshot snapshot1";

        private readonly List<SearchCriteriaQuery> m_conjunctionQuery;
        private readonly Dictionary<string, object> m_metadataParameters;
        private string m_headwordQueryParameter;

        public SearchCriteriaQueryCreator(List<SearchCriteriaQuery> conjunctionQuery,
            Dictionary<string, object> metadataParameters)
        {
            m_conjunctionQuery = conjunctionQuery;
            m_metadataParameters = metadataParameters;
        }

        public SortTypeEnumContract? Sort { get; set; }
        public SortDirectionEnumContract? SortDirection { get; set; }
        public int? Start { get; set; }
        public int? Count { get; set; }

        public int GetStart()
        {
            return Start ?? DefaultStart;
        }

        public int GetCount()
        {
            return Count != null ? Math.Min(Count.Value, MaxCount) : DefaultCount;
        }

        public Dictionary<string, object> GetMetadataParameters()
        {
            return m_metadataParameters;
        }

        public string GetQueryString()
        {
            var joinAndWhereClause = CreateJoinAndWhereClause(m_conjunctionQuery);
            var orderByClause = CreateOrderByClause("metadata1");

            var queryString = $"select metadata1 {ResultFromClause} where resource1.LatestVersion.Id = metadata1.Id and resource1.Project.Id in (select distinct project.Id {FromClause} {joinAndWhereClause}) {orderByClause}";

            return queryString;
        }

        public string GetProjectIdListQueryString()
        {
            var joinAndWhereClause = CreateJoinAndWhereClause(m_conjunctionQuery);
            
            var queryString = $"select distinct project.Id {FromClause} {joinAndWhereClause}";

            return queryString;
        }

        public string GetQueryStringForCount()
        {
            var joinAndWhereClause = CreateJoinAndWhereClause(m_conjunctionQuery);

            var queryString = $"select count(distinct metadata.Id) {FromClause} {joinAndWhereClause}";

            return queryString;
        }

        public string GetQueryStringForBookVersionPair()
        {
            var queryString =
                "select b.Guid as Guid, min(bv.VersionId) as VersionId from Book b inner join b.LastVersion bv";

            var whereClause = CreateJoinAndWhereClause(m_conjunctionQuery);

            queryString = string.Format("{0}{1} group by b.Guid", queryString, whereClause);

            return queryString;
        }

        public string GetQueryStringForIdList()
        {
            var queryString =
                "select b.Id from Book b inner join b.LastVersion bv";

            var whereClause = CreateJoinAndWhereClause(m_conjunctionQuery);

            queryString = string.Format("{0}{1} group by b.Id", queryString, whereClause);

            return queryString;
        }

        private string CreateJoinAndWhereClause(List<SearchCriteriaQuery> conjunctionQuery)
        {
            var joinBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();

            whereBuilder.Append(" where metadata.Id = resource.LatestVersion.Id");

            foreach (var criteriaQuery in conjunctionQuery)
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

        public string GetQueryStringForHeadwordCount()
        {
            var selectQueryString =
                "select b1.Id as BookId, count(distinct bh1.XmlEntryId) as HeadwordCount from Book b1 inner join b1.LastVersion bv1 inner join bv1.BookHeadwords bh1";

            selectQueryString = string.Format("{0} where b1.Id in ({1}) and bh1.Headword like :headwordQuery group by b1.Id", selectQueryString, GetQueryStringForIdList());
            return selectQueryString;
        }

        public string GetQueryStringForHeadwordList()
        {
            var selectQueryString =
                "select distinct b1.Guid as BookGuid, bv1.VersionId as BookVersionId, bv1.Title as BookTitle, bv1.Acronym as BookAcronym, bh1.DefaultHeadword as Headword, bh1.XmlEntryId as XmlEntryId, bh1.SortOrder as SortOrder, bh1.Image as Image from Book b1 inner join b1.LastVersion bv1 inner join bv1.BookHeadwords bh1";

            selectQueryString = string.Format("{0} where b1.Id in ({1}) and bh1.Headword like :headwordQuery order by bh1.SortOrder", selectQueryString, GetQueryStringForIdList());
            return selectQueryString;
        }

        public void SetHeadwordQueryParameter(string headwordQuery)
        {
            m_headwordQueryParameter = headwordQuery;
        }
    }

    public static class SearchCriteriaQueryCreatorExtensions
    {
        public static IQuery SetParameters(this IQuery query, SearchCriteriaQueryCreator creator)
        {
            var metadataParameters = creator.GetMetadataParameters();
            foreach (var parameterKeyValue in metadataParameters)
            {
                if (parameterKeyValue.Value is DateTime)
                {
                    //set parameter as DateTime2 otherwise comparison years before 1753 doesn't work
                    query.SetDateTime2(parameterKeyValue.Key, (DateTime)parameterKeyValue.Value);
                }
                else if (parameterKeyValue.Value is IList)
                {
                    query.SetParameterList(parameterKeyValue.Key, (IList)parameterKeyValue.Value);
                }
                else
                {
                    query.SetParameter(parameterKeyValue.Key, parameterKeyValue.Value);
                }
            }

            //if (m_headwordQueryParameter != null)
            //{
            //    query.SetParameter("headwordQuery", m_headwordQueryParameter);
            //}

            return query;
        }

        public static IQuery SetPaging(this IQuery query, SearchCriteriaQueryCreator creator)
        {
            query.SetFirstResult(creator.GetStart());
            query.SetMaxResults(creator.GetCount());

            return query;
        }
    }
}
