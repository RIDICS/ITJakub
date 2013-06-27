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
        private readonly ISearchResultProvider m_resultsProvider = new ItJakubSearchProvider();

        [HttpGet]
        public ActionResult Search(SearchViewModel model)
        {

            KeyWordsResponse response = m_resultsProvider.GetSearchResults(model.SearchTerm, ParseParamList(model.Kategorie), ParseParamList(model.Dila));

            List<string> parsedCategories = ParseParamList(model.Kategorie);
            List<string> parsedBooks = ParseParamList(model.Dila);

            return View(new SearchResultViewModel
                {
                    Search = new SearchViewModel
                        {
                            SearchTerm = model.SearchTerm,
                            Dila = model.Dila,
                            Kategorie = model.Kategorie
                        },
                    FoundWords = response.FoundTerms,
                    FoundInBooks = response.FoundInBooks,
                    Categories = response.CategoryTree,
                    SelectedCategoryIds = parsedCategories,
                    SelectedBookIds = parsedBooks,
                    CategoryIds = model.Kategorie,
                    BookIds = model.Dila,
                });
        }

        private List<string> ParseParamList(string paramList)
        {
            if (string.IsNullOrWhiteSpace(paramList)) return new List<string>();
            var splitted = paramList.Split(' ');
            return splitted.ToList();
        }

        [HttpGet]
        public ActionResult GetCategoryChildren(string categoryId)
        {
            List<SelectionBase> children;
            
            if (string.IsNullOrEmpty(categoryId))
                children = m_resultsProvider.GetRootCategories();
            else
                children = m_resultsProvider.GetCategoryChildrenById(categoryId);

            foreach (var category in children)
            {
                Category childAsCategory = category as Category;
                if (childAsCategory != null)
                {
                    childAsCategory.Subitems = new List<SelectionBase>();
                }
            }

            return View("GetCategoryChildren", null, new CategoriesViewModel { Children = children, CategoryId = categoryId });
        }

        [HttpGet]
        public ActionResult Detail(SearchViewModel model)
        {
            return View("Detail", null, new SearchKeyWordsViewModel
                {
                    Results = m_resultsProvider.GetHtmlContextForKeyWord(model.SearchTerm, ParseParamList(model.Kategorie), ParseParamList(model.Dila))
                });
        }

        [HttpGet]
        public ActionResult DetailByType(SearchViewModel model)
        {
            return View("DetailByType", null, model.BookId);
        }
    }
}