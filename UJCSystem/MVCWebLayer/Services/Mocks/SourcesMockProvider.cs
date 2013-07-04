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

        public IEnumerable<Book> GetSourcesTitleByLetter(string letter)
        {
            return new List<Book>();
        }

        public IEnumerable<Book> GetSourcesAuthorByLetter(string letter)
        {
            return new List<Book>();
        }
    }
}