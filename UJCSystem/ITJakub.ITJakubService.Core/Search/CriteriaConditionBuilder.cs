using System.Linq;
using System.Text;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;

namespace ITJakub.ITJakubService.Core.Search
{
    public static class CriteriaConditionBuilder
    {
        public static string Create(WordCriteriaContract word)
        {
            var stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(word.StartsWith))
            {
                stringBuilder.Append(word.StartsWith).Append("%");
            }

            if (word.Contains != null)
            {
                foreach (var innerWord in word.Contains.Where(innerWord => !string.IsNullOrEmpty(innerWord)))
                {
                    stringBuilder.Append("%").Append(innerWord).Append("%");
                }
            }

            if (!string.IsNullOrEmpty(word.EndsWith))
            {
                stringBuilder.Append("%").Append(word.EndsWith);
            }

            return stringBuilder.ToString();
        }
    }
}