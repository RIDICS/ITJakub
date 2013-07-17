using System.Collections.Generic;
using System.Web.Mvc;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.MVCWebLayer.Enums;
using ITJakub.MVCWebLayer.Services;
using ITJakub.MVCWebLayer.ViewModels;

namespace ITJakub.MVCWebLayer.Controllers
{
    public sealed class SourcesController : Controller
    {
        private readonly ISourcesProvider m_provider = new ItJakubSourcesProvider();

        [HttpGet]
        public ActionResult Detail(string id)
        {
            SearchResult searchResult = new SearchResult();
            if (id != "undefined")
            {
                searchResult = m_provider.GetDetail(id);
            }
            return View("Detail", searchResult);
        }

        [HttpGet]
        public ActionResult DetailPodminky(string id)
        {
            SearchResult searchResult = new SearchResult();
            if (id != "undefined")
            {
                searchResult = m_provider.GetDetail(id);
            }
            return View("DetailPodminky", searchResult);
        }

        [HttpGet]
        public ActionResult DetailZpracovani(string id)
        {
            SearchResult searchResult = new SearchResult();
            if (id != "undefined")
            {
                searchResult = m_provider.GetDetail(id);    
            }
            return View("DetailZpracovani", searchResult);
        }

        [HttpGet]
        public ActionResult DetailHledat(string id, string searchTerm)
        {
            if(string.IsNullOrEmpty(searchTerm))
                return View("DetailSearch", new SearchSourceDetailViewModel
                {
                    SearchTerm = string.Empty,
                    ShowResults = !string.IsNullOrEmpty(searchTerm),
                });
            var items = m_provider.GetHtmlContextForKeyWord(searchTerm, id);

            return View("DetailSearch", new SearchSourceDetailViewModel
                {
                    
                    SearchTerm = searchTerm,
                    ShowResults = !string.IsNullOrEmpty(searchTerm),
                    SearchResults = items,
                });
        }

        [HttpGet]
        public ActionResult Prochazet(string id, int page)
        {
            string detail = m_provider.GetContentByBookId(id);

            detail = "START Toto je content of book with id \"" + id +"\" END";
 
            return View("Prochazet", new SourcesContentViewModel 
            {
                Id = id,
                Page = page,
                PageCount = 87 /*TODO get page count*/,
                Content = detail
            });
        }

        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            return View("Search", new SearchSourcesViewModel
                {
                    FoundSources = m_provider.GetSearchResult(searchTerm),
                });
        }

        [HttpGet]
        public ActionResult Listing(string alphabet, string mode)
        {
            alphabet = alphabet.ToUpper();
            if (string.IsNullOrEmpty(alphabet))
            {
                alphabet = "A";
            }

            SourcesViewType viewType = SourcesViewModeConverter.FromUrlParam(mode);

            IEnumerable<SearchResult> results = new List<SearchResult>();
            switch (viewType)
            {
                case SourcesViewType.Author:
                    results = m_provider.GetSourcesAuthorByLetter(alphabet);
                    break;
                case SourcesViewType.Name:
                    results = m_provider.GetSourcesTitleByLetter(alphabet);
                    break;
            }

            return View(new ListSourcesViewModel
                {
                    ViewType = SourcesViewModeConverter.FromUrlParam(mode),
                    FoundSources = results,
                });
        }
    };
}