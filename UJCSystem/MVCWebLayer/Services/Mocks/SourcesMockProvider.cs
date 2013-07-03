using System;
using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.MVCWebLayer.Enums;

namespace ITJakub.MVCWebLayer.Services.Mocks
{
    public class MockSourcesProvider : ISourcesProvider
    {
        public IEnumerable<SearchResult> GetSearchResult(string searchTerm)
        {
            return new List<SearchResult>
                {
                    { new SearchResult {Title = "Bible olomoucká, Genesis–Esdráš", Author = ""} }
                };
        }

        public Book GetDetail(string id)
        {
            return new Book { Name = "Bible olomoucká, Genesis–Esdráš", Author = "" };
        }

        public IEnumerable<Book> GetSources(string query, SourcesViewMode mode)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            query = query.Trim();

            if (string.Equals(query, "A", StringComparison.InvariantCultureIgnoreCase) && mode == SourcesViewMode.Name)
            {
                return new List<Book>
                {
                    { new Book{ Name = "[Alexandreida. Zlomek budějovicko-muzejní.]", Author = ""} },
                    { new Book{ Name = "[Alexandreida. Zlomek budějovický.]"} },
                    { new Book{ Name = "[Alexandreida. Zlomek budějovický druhý.]"} }
                };
            }
            else if (string.Equals(query, "B", StringComparison.InvariantCultureIgnoreCase) && mode == SourcesViewMode.Name)
            {
                return new List<Book>
                {
                    { new Book{ Name = "[Bible kladrubská, kniha Jozue]", Author = ""} },
                    { new Book{ Name = "	[Bible olomoucká, Genesis–Esdráš]", Author = ""} }
                };
            }
            else if (string.Equals(query, "B", StringComparison.InvariantCultureIgnoreCase) && mode == SourcesViewMode.Author)
            {
                return new List<Book>
                {
                    { new Book{ Name = "[Sborník traktátů Jana Bechyňky (neuberský)]", Author = "Bechyňka, Jan"} },
                    { new Book{ Name = "Překlad kroniky Twingerovy", Author = "Beneš z Hořovic"} }
                };
            }

            return new List<Book>();
        }
    }
}