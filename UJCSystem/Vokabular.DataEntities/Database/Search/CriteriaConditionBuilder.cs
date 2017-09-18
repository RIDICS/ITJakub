using System.Linq;
using System.Text;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;

namespace Vokabular.DataEntities.Database.Search
{
    public static class CriteriaConditionBuilder
    {
        private const string AnyStringWildcard = "%";

        public static string Create(WordCriteriaContract word)
        {
            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(word.ExactMatch))
            {
                stringBuilder.Append(word.ExactMatch);
            }

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

            // Escape unwanted characters
            stringBuilder.Replace("[", "[[]");

            return stringBuilder.ToString();
        }
    }
}