using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;

namespace ITJakub.SearchService.Core.Search
{
    public static class RegexCriteriaBuilder
    {
        private static string ConvertWildcardToRegex(string stringWithWildcard)
        {
            return stringWithWildcard == null
                ? null
                : stringWithWildcard.Replace(".", "\\.")
                    .Replace("_", ".")
                    .Replace("%", ".*");
        }

        private static void ConvertWildcardToRegex(WordCriteriaContract wordCriteria)
        {
            wordCriteria.StartsWith = ConvertWildcardToRegex(wordCriteria.StartsWith);
            wordCriteria.EndsWith = ConvertWildcardToRegex(wordCriteria.EndsWith);

            if (wordCriteria.Contains != null)
                wordCriteria.Contains = wordCriteria.Contains.Select(ConvertWildcardToRegex).ToList();
        }

        public static void ConvertWildcardToRegex(IList<SearchCriteriaContract> searchCriterias)
        {
            foreach (var searchCriteria in searchCriterias)
            {
                if (searchCriteria is WordListCriteriaContract)
                {
                    var wordListCriteria = (WordListCriteriaContract) searchCriteria;
                    foreach (var wordCriteria in wordListCriteria.Disjunctions)
                    {
                        ConvertWildcardToRegex(wordCriteria);
                    }
                }
                else if (searchCriteria is TokenDistanceListCriteriaContract)
                {
                    var tokenDistaceList = (TokenDistanceListCriteriaContract) searchCriteria;
                    foreach (var tokenDistance in tokenDistaceList.Disjunctions)
                    {
                        ConvertWildcardToRegex(tokenDistance.First);
                        ConvertWildcardToRegex(tokenDistance.Second);
                    }
                }
            }
        }

        private static string CreateRegex(WordCriteriaContract word)
        {
            var regexBuilder = new StringBuilder();

            regexBuilder.Append("^");
            if (!string.IsNullOrEmpty(word.StartsWith))
            {
                regexBuilder.Append("(").Append(word.StartsWith).Append(").*");
            }

            if (word.Contains != null)
            {
                foreach (var innerWord in word.Contains.Where(innerWord => !string.IsNullOrEmpty(innerWord)))
                {
                    regexBuilder.Append(".*(").Append(innerWord).Append(").*");
                }
            }

            if (!string.IsNullOrEmpty(word.EndsWith))
            {
                regexBuilder.Append(".*(").Append(word.EndsWith).Append(")");
            }
            regexBuilder.Append("$");

            return regexBuilder.ToString();
        }

        public static SearchCriteriaContract ConvertToRegexCriteria(SearchCriteriaContract searchCriteriaContract)
        {
            if (searchCriteriaContract is WordListCriteriaContract)
            {
                return ConvertToRegexCriteria((WordListCriteriaContract)searchCriteriaContract);
            }
            if (searchCriteriaContract is TokenDistanceListCriteriaContract)
            {
                return ConvertToRegexCriteria((TokenDistanceListCriteriaContract)searchCriteriaContract);
            }
            return searchCriteriaContract;
        }

        private static RegexSearchCriteriaContract ConvertToRegexCriteria(WordListCriteriaContract criteriaContract)
        {
            var regexList = new List<string>();
            foreach (var word in criteriaContract.Disjunctions)
            {
                regexList.Add(CreateRegex(word));
            }

            return new RegexSearchCriteriaContract
            {
                Key = criteriaContract.Key,
                Disjunctions = regexList
            };
        }

        private static RegexTokenListCriteriaContract ConvertToRegexCriteria(TokenDistanceListCriteriaContract criteriaContract)
        {
            var tokenList = new List<RegexTokenDistanceCriteriaContract>();
            foreach (var tokenDistanceCriteria in criteriaContract.Disjunctions)
            {
                var regexTokenDistance = new RegexTokenDistanceCriteriaContract
                {
                    Distance = tokenDistanceCriteria.Distance,
                    FirstRegex = CreateRegex(tokenDistanceCriteria.First),
                    SecondRegex = CreateRegex(tokenDistanceCriteria.Second)
                };
                tokenList.Add(regexTokenDistance);
            }

            return new RegexTokenListCriteriaContract
            {
                Key = criteriaContract.Key,
                Disjunctions = tokenList
            };
        }
    }
}