using System.Collections.Generic;
using ITJakub.Contracts.Searching;
using Ujc.Naki.MVCWebLayer.Services.Enums;

namespace Ujc.Naki.MVCWebLayer.Services.Mocks
{
    public class SearchResultsMockProvider : ISearchResultProvider
    {
        private static readonly string[] m_pesQueryResults = new string[] 
            { "pes", "Pes", "peskovati", "peskovati sě", "pesky", "peský", "pessky", "pesský" };
        private static readonly string[] m_janQueryResults = new string[] { "Jan" };

        private static Dictionary<BookCategory, List<string>> PesQueryResultsByType = new Dictionary<BookCategory,List<string>>()
        {
            { BookCategory.Dictionary, new List<string> { "Malý staročeský slovník (MSS)",  "Jan Gebauer: Slovník staročeský (GbSlov)", "Staročeský slovník (StčS)" }},
            { BookCategory.Historic, new List<string> { "Alexandreida. Zlomek budějovický druhý" , "Bible kladrubská, kniha Jozue", "Bible olomoucká, Genesis–Esdráš"}}
        };
        private static Dictionary<BookCategory, List<string>> JanQueryResultsByType = new Dictionary<BookCategory,List<string>>()
        {
            { BookCategory.Dictionary, new List<string> { "Jan Gebauer: Slovník staročeský (GbSlov)", "Staročeský slovník (StčS)" }}
        };

        public string[] GetSearchResults(string query)
        {
            if (string.Equals(query, "pes*"))
            {
                return m_pesQueryResults;
            }
            else
            {
                return m_janQueryResults;
            }
        }

        public Dictionary<BookCategory, List<string>> GetSearchResultsByType(string query)
        {
            if (string.Equals(query, "pes*"))
            {
                return PesQueryResultsByType;
            }
            else
            {
                return JanQueryResultsByType;
            }
        }

        public SearchResult[] GetKwicForKeyWord(string searchTerm)
        {
            return new SearchResult[0];
        }
    }
}