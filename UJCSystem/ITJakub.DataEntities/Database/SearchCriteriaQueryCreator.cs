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

        public SearchCriteriaQueryCreator(List<SearchCriteriaQuery> conjunctionQuery,
            Dictionary<string, object> metadataParameters)
        {
            m_conjunctionQuery = conjunctionQuery;
            m_metadataParameters = metadataParameters;
        }

        public string GetQueryString()
        {
            var queryString =
                "select b.Guid as Guid, min(bv.VersionId) as VersionId from Book b inner join b.LastVersion bv";
            var joinBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();
            foreach (var criteriaQuery in m_conjunctionQuery)
            {
                if (!string.IsNullOrEmpty(criteriaQuery.Join))
                    joinBuilder.Append(' ').Append(criteriaQuery.Join);

                whereBuilder.Append(whereBuilder.Length > 0 ? " and" : " where");

                whereBuilder.Append(" (").Append(criteriaQuery.Where).Append(')');
            }

            queryString = string.Format("{0}{1}{2} group by b.Guid", queryString, joinBuilder, whereBuilder);

            return queryString;
        }

        public void SetParameters(IQuery query)
        {
            foreach (var parameterKeyValue in m_metadataParameters)
            {
                if (parameterKeyValue.Value is DateTime)
                    //set parameter as DateTime2 otherwise comparison years before 1753 doesn't work
                    query.SetDateTime2(parameterKeyValue.Key, (DateTime) parameterKeyValue.Value);
                else if (parameterKeyValue.Value is IList)
                {
                    query.SetParameterList(parameterKeyValue.Key, (IList) parameterKeyValue.Value);
                }
                else
                {
                    query.SetParameter(parameterKeyValue.Key, parameterKeyValue.Value);
                }
            }
        }
    }
}