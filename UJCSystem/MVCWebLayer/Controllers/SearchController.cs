using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITJakub.Contracts.Searching;
using Ujc.Naki.MVCWebLayer.Services;
using Ujc.Naki.MVCWebLayer.Services.Mocks;
using Ujc.Naki.MVCWebLayer.ViewModels;

namespace Ujc.Naki.MVCWebLayer.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookCategoryService m_service = new BookCategoryService();
        //private readonly ISearchResultProvider m_resultsProvider = new SearchResultsMockProvider();
        private readonly ISearchResultProvider m_resultsProvider = new ItJakubSearchProvider();

        [HttpGet]
        public ActionResult Search(SearchViewModel model)
        {
            return View(new SearchResultViewModel
            {
                Search = new SearchViewModel
                    {
                        SearchTerm = model.SearchTerm,
                        SearchPart = model.SearchPart,
                        SearchedBooks = m_service.GetSearchedBooks(model.SearchPart),
                    },
                FoundWords = m_resultsProvider.GetSearchResults(model.SearchTerm),
                FoundByType = m_resultsProvider.GetSearchResultsByType(model.SearchTerm),
            });
        }

        [HttpGet]
        public ActionResult Detail(string searchTerm)
        {
            return View("Detail", null,new SearchKeyWordsViewModel
                {
                    Results = m_resultsProvider.GetKwicForKeyWord(searchTerm)
                });
        }

        [HttpGet]
        public ActionResult DetailByType(string book)
        {
            return View("DetailByType", null, book);
        }
    }
}
