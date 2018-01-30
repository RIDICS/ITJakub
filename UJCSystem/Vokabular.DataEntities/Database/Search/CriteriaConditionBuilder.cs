using System.Linq;
using System.Text;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;

namespace Vokabular.DataEntities.Database.Search
{
    public static class CriteriaConditionBuilder
    {
        private const string AnyStringWildcard = NHibernateDao.WildcardAny;

        public static string Create(WordCriteriaContract word)
        {
            if (!string.IsNullOrEmpty(word.ExactMatch))
            {
                return NHibernateDao.EscapeQuery(word.ExactMatch);
            }

            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(word.StartsWith))
            {
                stringBuilder.Append(word.StartsWith).Append(AnyStringWildcard);
            }

            if (word.Contains != null)
            {
                foreach (var innerWord in word.Contains.Where(innerWord => !string.IsNullOrEmpty(innerWord)))
                {
                    stringBuilder.Append(AnyStringWildcard).Append(innerWord).Append(AnyStringWildcard);
                }
            }

            if (!string.IsNullOrEmpty(word.EndsWith))
            {
                stringBuilder.Append(AnyStringWildcard).Append(word.EndsWith);
            }
            
            var escapedValue = NHibernateDao.EscapeQuery(stringBuilder.ToString());
            return escapedValue;
        }
    }
}