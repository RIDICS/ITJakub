using System.Collections.Generic;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Favorites;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models.Favorite;
using ITJakub.Web.Hub.Models.Requests.Favorite;
using Localization.AspNetCore.Service;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class FavoriteController : BaseController
    {
        private const int AllFavoriteCount = 0;
        private const int LatestFavoriteCount = 5;
        private readonly ILocalization m_localizer;

        public FavoriteController(CommunicationProvider communicationProvider, ILocalization localizer) : base(communicationProvider)
        {
            m_localizer = localizer;
        }
        
        public ActionResult Management()
        {
            using (var client = GetMainServiceClient())
            {
                var favoriteLabels = client.GetFavoriteLabels(AllFavoriteCount);
                var viewModel = new FavoriteManagementViewModel
                {
                    FavoriteLabels = Mapper.Map<IList<FavoriteLabelViewModel>>(favoriteLabels),
                    SortList = new List<FavoriteSortViewModel>
                    {
                        new FavoriteSortViewModel(FavoriteSortContract.TitleAsc, m_localizer.Translate("TitleAsc", "Favorite")),
                        new FavoriteSortViewModel(FavoriteSortContract.TitleDesc, m_localizer.Translate("TitleDesc", "Favorite")),
                        new FavoriteSortViewModel(FavoriteSortContract.CreateTimeAsc, m_localizer.Translate("CreateTimeAsc", "Favorite")),
                        new FavoriteSortViewModel(FavoriteSortContract.CreateTimeDesc, m_localizer.Translate("CreateTimeDesc", "Favorite"))
                    },
                    FilterList = new List<FavoriteFilterViewModel>
                    {
                        new FavoriteFilterViewModel(FavoriteTypeContract.Unknown, m_localizer.Translate("All", "Favorite")),
                        new FavoriteFilterViewModel(FavoriteTypeContract.Book, m_localizer.Translate("Books", "Favorite")),
                        new FavoriteFilterViewModel(FavoriteTypeContract.Category, m_localizer.Translate("Category", "Favorite")),
                        new FavoriteFilterViewModel(FavoriteTypeContract.PageBookmark, m_localizer.Translate("PageBookmark", "Favorite")),
                        new FavoriteFilterViewModel(FavoriteTypeContract.Query, m_localizer.Translate("Query", "Favorite"))
                    }
                };
                return View("FavoriteManagement", viewModel);
            }
        }

        public ActionResult NewFavorite(string itemName)
        {
            if (itemName == null)
                itemName = string.Empty;

            if (!IsUserLoggedIn())
            {
                var viewModel = new NewFavoriteViewModel
                {
                    ItemName = itemName,
                    Labels = new List<FavoriteLabelViewModel>
                    {
                        new FavoriteLabelViewModel
                        {
                            Id = 0,
                            Name = "Všechny položky",
                            Color = "#CC9900",
                            IsDefault = true,
                            LastUseTime = null
                        }
                    }
                };
                return PartialView("_NewFavorite", viewModel);
            }

            using (var client = GetMainServiceClient())
            {
                var favoriteLabels = client.GetFavoriteLabels(AllFavoriteCount);

                var favoriteLabelViewModels = Mapper.Map<IList<FavoriteLabelViewModel>>(favoriteLabels);
                var viewModel = new NewFavoriteViewModel
                {
                    ItemName = itemName,
                    Labels = favoriteLabelViewModels
                };

                return PartialView("_NewFavorite", viewModel);
            }
        }

        public ActionResult Favorite(long id)
        {
            using (var client = GetMainServiceClient())
            {
                var favoriteItem = client.GetFavoriteItem(id);

                switch (favoriteItem.FavoriteType)
                {
                    case FavoriteTypeContract.Book:
                        return RedirectToAction("Listing", "Editions", new {area = "Editions", bookId = favoriteItem.Book.Guid});
                    case FavoriteTypeContract.Category:
                        return View("AmbiguousFavoriteRedirect");
                    case FavoriteTypeContract.PageBookmark:
                        return RedirectToAction("Listing", "Editions", new { area = "Editions", bookId = favoriteItem.Book.Guid, page = favoriteItem.PageXmlId });
                    case FavoriteTypeContract.Query:
                        return RedirectToFavoriteQuery(favoriteItem.BookType, favoriteItem.QueryType, favoriteItem.Query);
                    default:
                        return RedirectToAction("Management");
                }
            }
        }
        
        public ActionResult Dialog()
        {
            return PartialView("Plugins/_Dialog");
        }

        private ActionResult RedirectToFavoriteQuery(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string query)
        {
            string area = null;
            string action = null;

            switch (bookType)
            {
                case BookTypeEnumContract.AudioBook:
                    area = "AudioBooks";
                    break;
                case BookTypeEnumContract.BibliographicalItem:
                    area = "Bibliographies";
                    break;
                case BookTypeEnumContract.CardFile:
                    area = "CardFiles";
                    break;
                case BookTypeEnumContract.Dictionary:
                    area = "Dictionaries";
                    break;
                case BookTypeEnumContract.Edition:
                    area = "Editions";
                    break;
                case BookTypeEnumContract.Grammar:
                    area = "OldGrammar";
                    break;
                case BookTypeEnumContract.ProfessionalLiterature:
                    area = "ProfessionalLiterature";
                    break;
                case BookTypeEnumContract.TextBank:
                    area = "BohemianTextBank";
                    break;
            }

            switch (queryType)
            {
                case QueryTypeEnumContract.Search:
                    action = "Search";
                    break;
                case QueryTypeEnumContract.List:
                    action = "List";
                    break;
                case QueryTypeEnumContract.Reader:
                    return View("AmbiguousFavoriteRedirect");
            }

            return RedirectToAction(action, area, new {area = area, search = query});
        }
        
        [HttpPost]
        public ActionResult GetFavoriteLabeledBooks([FromBody] GetFavoriteLabeledBookRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabeledBooks(request.BookIds);
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult GetFavoriteLabeledCategories([FromBody] GetFavoriteLabeledCategoryRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabeledCategories(request.CategoryIds);
                return Json(result);
            }
        }

        public ActionResult GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabelsWithBooksAndCategories(bookType);
                return Json(result);
            }
        }

        public ActionResult CreateFavoriteBook([FromBody] CreateFavoriteBookRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var resultIds = client.CreateFavoriteBook(request.BookId, request.Title, request.LabelIds);
                return Json(resultIds);
            }
        }

        public ActionResult CreateFavoriteCategory([FromBody] CreateFavoriteCategoryRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var resultIds = client.CreateFavoriteCategory(request.CategoryId, request.Title, request.LabelIds);
                return Json(resultIds);
            }
        }

        public ActionResult CreateFavoriteQuery([FromBody] CreateFavoriteQueryRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var resultIds = client.CreateFavoriteQuery(request.BookType, request.QueryType, request.Query, request.Title, request.LabelIds);
                return Json(resultIds);
            }
        }

        public ActionResult CreatePageBookmark([FromBody] CreatePageBookmarkRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var resultIds = client.CreatePageBookmark(request.BookXmlId, request.PageXmlId, request.Title, request.LabelIds);
                return Json(resultIds);
            }
        }

        public ActionResult GetPageBookmarks(string bookXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetPageBookmarks(bookXmlId);
                return Json(result);
            }
        }

        public ActionResult GetFavoriteQueries(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle, int start, int count)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteQueries(labelId, bookType, queryType, filterByTitle, start, count);
                return Json(result);
            }
        }

        public ActionResult GetFavoriteQueriesCount(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteQueriesCount(labelId, bookType, queryType, filterByTitle);
                return Json(result);
            }
        }

        public ActionResult GetLabelList()
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabels(AllFavoriteCount);
                return Json(result);
            }
        }

        public ActionResult GetLatestLabelList()
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabels(LatestFavoriteCount);
                return Json(result);
            }
        }

        public ActionResult GetFavoriteList(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle, FavoriteSortContract? sort, int start, int count)
        {
            if (sort == null)
                sort = FavoriteSortContract.TitleAsc;

            if (filterByType != null && filterByType.Value == FavoriteTypeContract.Unknown)
                filterByType = null;

            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteItems(labelId, filterByType, filterByTitle, sort.Value, start, count);
                return Json(result);
            }
        }

        public ActionResult GetFavoriteListCount(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle)
        {
            if (filterByType != null && filterByType.Value == FavoriteTypeContract.Unknown)
                filterByType = null;

            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteItemsCount(labelId, filterByType, filterByTitle);
                return Json(result);
            }
        }
        
        public ActionResult GetFavoriteLabelManagementPartial(long id, string name, string color)
        {
            var viewModel = new FavoriteLabelViewModel
            {
                Id = id,
                Name = name,
                Color = color,
                IsDefault = false
            };

            return PartialView("_FavoriteLabelManagement", viewModel);
        }

        public ActionResult CreateLabel([FromBody] CreateLabelRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var resultId = client.CreateFavoriteLabel(request.Name, request.Color);
                return Json(resultId);
            }
        }

        public ActionResult UpdateLabel([FromBody] UpdateLabelRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.UpdateFavoriteLabel(request.LabelId, request.Name, request.Color);
                return Json(new { });
            }
        }

        public ActionResult DeleteLabel([FromBody] DeleteLabelRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.DeleteFavoriteLabel(request.LabelId);
                return Json(new {});
            }
        }

        public ActionResult UpdateFavoriteItem([FromBody] UpdateFavoriteItemRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.UpdateFavoriteItem(request.Id, request.Title);
                return Json(new {});
            }
        }

        public ActionResult DeleteFavoriteItem([FromBody] DeleteFavoriteItemRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                client.DeleteFavoriteItem(request.Id);
                return Json(new {});
            }
        }
    }
}