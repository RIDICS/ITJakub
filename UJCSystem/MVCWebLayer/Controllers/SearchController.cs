using System.Web.Mvc;
using ITJakub.Contracts.Categories;
using ITJakub.MVCWebLayer.Services;
using ITJakub.MVCWebLayer.ViewModels;

namespace ITJakub.MVCWebLayer.Controllers
{
    public class SearchController : Controller
    {
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
                        },
                    FoundWords = m_resultsProvider.GetSearchResults(model.SearchTerm),
                });
        }

        [HttpGet]
        public ActionResult GetCategoryChildren(string categoryId)
        {
            SelectionBase[] children;
            
            if (string.IsNullOrEmpty(categoryId))
                children = m_resultsProvider.GetRootCategories();
            else
                children = m_resultsProvider.GetCategoryChildrenById(categoryId);


            return View("GetCategoryChildren", null, new CategoriesViewModel {Children = children});
        }

        [HttpGet]
        public ActionResult Detail(string searchTerm)
        {
            return View("Detail", null, new SearchKeyWordsViewModel
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