using System;
using System.Collections;
using NHibernate;

namespace Vokabular.DataEntities.Database.Search
{
    public static class SearchCriteriaQueryCreatorExtensions
    {
        public static IQuery SetParameters(this IQuery query, ISearchCriteriaCreator creator)
        {
            var metadataParameters = creator.Parameters;
            foreach (var parameterKeyValue in metadataParameters)
            {
                if (parameterKeyValue.Value is DateTime)
                {
                    //since NHibernate 5.0 DateTime2 is default type
                    query.SetDateTime(parameterKeyValue.Key, (DateTime)parameterKeyValue.Value);
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