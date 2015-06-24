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

        public SearchResult GetDetail(string id)
        {
            return new SearchResult { Title = "Bible olomoucká, Genesis–Esdráš", Author = "" };
        }

        public IEnumerable<SearchResult> GetSourcesTitleByLetter(string letter)
        {
            return new List<SearchResult>();
        }

        public IEnumerable<SearchResult> GetSourcesAuthorByLetter(string letter)
        {
            return new List<SearchResult>();
        }

        public string GetContentByBookId(string id)
        {
            return string.Empty;
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string searchTerm, string booksIds)
        {
            return new List<SearchResultWithHtmlContext>();
        }
    }
}