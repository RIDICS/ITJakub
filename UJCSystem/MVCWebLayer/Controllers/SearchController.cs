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
        public ActionResult Search(SearchViewModel model, string searchTerm, string kategorie, string dila)
        {
            return View(new SearchResultViewModel
                {
                    Search = new SearchViewModel
                        {
                            SearchTerm = model.SearchTerm,
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


            return View("GetCategoryChildren", null, new CategoriesViewModel { Children = children, CategoryId = categoryId });
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
        public ActionResult DetailByType(string category)
        {
            return View("DetailByType", null, category);
        }
    }
}