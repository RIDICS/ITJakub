using System.Collections.Generic;
using System.Linq;
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

            KeyWordsResponse response = m_resultsProvider.GetSearchResults(model.SearchTerm, ParseParamList(model.Kategorie), ParseParamList(model.Dila));

            return View(new SearchResultViewModel
                {
                    Search = new SearchViewModel
                        {
                            SearchTerm = model.SearchTerm,
                            Dila = model.Dila,
                            Kategorie = model.Kategorie
                        },
                    FoundWords = response.FoundTerms,
                    Categories = response.CategoryTree,
                    SelectedCategoryIds = model.Kategorie,
                    SelectedBookIds = model.Dila,
                });
        }

        private List<string> ParseParamList(string paramList)
        {
            if (paramList == null) return new List<string>();
            var splitted = paramList.Split(' ');
            return splitted.ToList();
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