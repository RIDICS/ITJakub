using System.Linq;
using System.Text;
using ITJakub.Shared.Contracts.Searching;

namespace ITJakub.SearchService.Core.Search
{
    public static class RegexCriteriaBuilder
    {
        public static RegexWordCriteriaContract ConvertToRegexCriteria(WordListCriteriaContract criteriaContract)
        {
            var regexBuilder = new StringBuilder();
            foreach (var word in criteriaContract.Values)
            {
                if (regexBuilder.Length > 0)
                {
                    regexBuilder.Append("|");
                }

                if (!string.IsNullOrEmpty(word.StartsWith))
                {
                    regexBuilder.Append(word.StartsWith).Append(".*");
                }

                if (word.Contains != null)
                {
                    foreach (var innerWord in word.Contains.Where(innerWord => !string.IsNullOrEmpty(innerWord)))
                    {
                        regexBuilder.Append(".*").Append(innerWord).Append(".*");
                    }
                }

                if (!string.IsNullOrEmpty(word.EndsWith))
                {
                    regexBuilder.Append(".*").Append(word.EndsWith);
                }
            }

            return new RegexWordCriteriaContract
            {
                Key = criteriaContract.Key,
                RegexContent = regexBuilder.ToString()
            };
        }
    }
}