using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace ITJakub.DataEntities.Database
{
    public class SearchCriteriaQueryCreator
    {
        private readonly List<SearchCriteriaQuery> m_conjunctionQuery;
        private readonly Dictionary<string, object> m_metadataParameters;
        private string m_headwordQueryParameter;

        public SearchCriteriaQueryCreator(List<SearchCriteriaQuery> conjunctionQuery,
            Dictionary<string, object> metadataParameters)
        {
            m_conjunctionQuery = conjunctionQuery;
            m_metadataParameters = metadataParameters;
        }

        public string GetQueryStringForBookVersionPair()
        {
            var queryString =
                "select b.Guid as Guid, min(bv.VersionId) as VersionId from Book b inner join b.LastVersion bv";

            var whereClause = CreateWhereClauseForQueryString(m_conjunctionQuery);

            queryString = string.Format("{0}{1} group by b.Guid", queryString, whereClause);

            return queryString;
        }

        public string GetQueryStringForIdList()
        {
            var queryString =
                "select b.Id from Book b inner join b.LastVersion bv";

            var whereClause = CreateWhereClauseForQueryString(m_conjunctionQuery);

            queryString = string.Format("{0}{1} group by b.Id", queryString, whereClause);

            return queryString;
        }

        private string CreateWhereClauseForQueryString(List<SearchCriteriaQuery> conjunctionQuery)
        {
            var joinBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();
            foreach (var criteriaQuery in conjunctionQuery)
            {
                if (!string.IsNullOrEmpty(criteriaQuery.Join))
                    joinBuilder.Append(' ').Append(criteriaQuery.Join);

                whereBuilder.Append(whereBuilder.Length > 0 ? " and" : " where");

                whereBuilder.Append(" (").Append(criteriaQuery.Where).Append(')');
            }

            return string.Format("{0}{1}", joinBuilder, whereBuilder);
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

        public void SetParameters(IQuery query)
        {
            foreach (var parameterKeyValue in m_metadataParameters)
            {
                if (parameterKeyValue.Value is DateTime)
                {
                    //set parameter as DateTime2 otherwise comparison years before 1753 doesn't work
                    query.SetDateTime2(parameterKeyValue.Key, (DateTime)parameterKeyValue.Value);
                }
                else if (parameterKeyValue.Value is IList)
                {
                    query.SetParameterList(parameterKeyValue.Key, (IList) parameterKeyValue.Value);
                }
                else
                {
                    query.SetParameter(parameterKeyValue.Key, parameterKeyValue.Value);
                }
            }

            if (m_headwordQueryParameter != null)
            {
                query.SetParameter("headwordQuery", m_headwordQueryParameter);
            }
        }
    }
}