using System.Collections.Generic;
using AutoMapper;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models.Favorite;
using ITJakub.Web.Hub.Models.Requests.Favorite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Types.Favorite;

namespace ITJakub.Web.Hub.Controllers
{
    public class FavoriteController : BaseController
    {
        private const int LatestFavoriteCount = 5;
        private readonly ILocalizationService m_localizer;

        public FavoriteController(CommunicationProvider communicationProvider, ILocalizationService localizer) : base(communicationProvider)
        {
            m_localizer = localizer;
        }

        public ActionResult Management()
        {
            var client = GetFavoriteClient();

            var favoriteLabels = client.GetFavoriteLabelList();
            var viewModel = new FavoriteManagementViewModel
            {
                FavoriteLabels = Mapper.Map<IList<FavoriteLabelViewModel>>(favoriteLabels),
                SortList = new List<FavoriteSortViewModel>
                {
                    new FavoriteSortViewModel(FavoriteSortEnumContract.TitleAsc, m_localizer.Translate("TitleAsc", "Favorite")),
                    new FavoriteSortViewModel(FavoriteSortEnumContract.TitleDesc, m_localizer.Translate("TitleDesc", "Favorite")),
                    new FavoriteSortViewModel(FavoriteSortEnumContract.CreateTimeAsc, m_localizer.Translate("CreateTimeAsc", "Favorite")),
                    new FavoriteSortViewModel(FavoriteSortEnumContract.CreateTimeDesc, m_localizer.Translate("CreateTimeDesc", "Favorite"))
                },
                FilterList = new List<FavoriteFilterViewModel>
                {
                    new FavoriteFilterViewModel(FavoriteTypeEnumContract.Unknown, m_localizer.Translate("All", "Favorite")),
                    new FavoriteFilterViewModel(FavoriteTypeEnumContract.Project, m_localizer.Translate("Books", "Favorite")),
                    new FavoriteFilterViewModel(FavoriteTypeEnumContract.Category, m_localizer.Translate("Category", "Favorite")),
                    new FavoriteFilterViewModel(FavoriteTypeEnumContract.Page, m_localizer.Translate("PageBookmark", "Favorite")),
                    new FavoriteFilterViewModel(FavoriteTypeEnumContract.Query, m_localizer.Translate("Query", "Favorite"))
                }
            };
            return View("FavoriteManagement", viewModel);
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
            else
            {
                var client = GetFavoriteClient();
                var favoriteLabels = client.GetFavoriteLabelList();

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
            var client = GetFavoriteClient();
            var favoriteItem = client.GetFavoriteItem(id);

            switch (favoriteItem.FavoriteType)
            {
                case FavoriteTypeEnumContract.Project:
                    return RedirectToAction("Listing", "Editions", new {area = "Editions", bookId = favoriteItem.ProjectId});
                case FavoriteTypeEnumContract.Category:
                    return View("AmbiguousFavoriteRedirect");
                case FavoriteTypeEnumContract.Page:
                    return RedirectToAction("Listing", "Editions",
                        new {area = "Editions", bookId = favoriteItem.ProjectId, page = favoriteItem.PageId});
                case FavoriteTypeEnumContract.Query:
                    if (favoriteItem.BookType == null || favoriteItem.QueryType == null)
                    {
                        return StatusCode(StatusCodes.Status502BadGateway, "Invalid response from service");
                    }

                    return RedirectToFavoriteQuery(favoriteItem.BookType.Value, favoriteItem.QueryType.Value, favoriteItem.Query);
                default:
                    return RedirectToAction("Management");
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
            var client = GetFavoriteClient();
            var result = client.GetFavoriteLabeledBooks(request.BookIds, request.BookType);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetFavoriteLabeledCategories([FromBody] GetFavoriteLabeledCategoryRequest request)
        {
            var client = GetFavoriteClient();
            var result = client.GetFavoriteLabeledCategories();
            return Json(result);
        }

        public ActionResult GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
        {
            var client = GetFavoriteClient();
            var result = client.GetFavoriteLabelsWithBooksAndCategories(bookType);
            return Json(result);
        }

        public ActionResult CreateFavoriteBook([FromBody] CreateFavoriteBookRequest request)
        {
            var client = GetFavoriteClient();
            var resultIds = new List<long>();
            foreach (var requestLabelId in request.LabelIds)
            {
                var resultId = client.CreateFavoriteBook(new CreateFavoriteProjectContract
                {
                    ProjectId = request.BookId,
                    FavoriteLabelId = requestLabelId,
                    Title = request.Title,
                });
                resultIds.Add(resultId);
            }

            return Json(resultIds);
        }

        public ActionResult CreateFavoriteCategory([FromBody] CreateFavoriteCategoryRequest request)
        {
            var client = GetFavoriteClient();
            var resultIds = new List<long>();
            foreach (var requestLabelId in request.LabelIds)
            {
                var resultId = client.CreateFavoriteCategory(new CreateFavoriteCategoryContract
                {
                    CategoryId = request.CategoryId,
                    FavoriteLabelId = requestLabelId,
                    Title = request.Title,
                });
                resultIds.Add(resultId);
            }

            return Json(resultIds);
        }

        public ActionResult CreateFavoriteQuery([FromBody] CreateFavoriteQueryRequest request)
        {
            var client = GetFavoriteClient();
            var resultIds = new List<long>();
            foreach (var requestLabelId in request.LabelIds)
            {
                var resultId = client.CreateFavoriteQuery(new CreateFavoriteQueryContract
                {
                    Title = request.Title,
                    BookType = request.BookType,
                    FavoriteLabelId = requestLabelId,
                    Query = request.Query,
                    QueryType = request.QueryType,
                });
                resultIds.Add(resultId);
            }

            return Json(resultIds);
        }

        public ActionResult CreatePageBookmark([FromBody] CreatePageBookmarkRequest request)
        {
            var client = GetFavoriteClient();
            var resultIds = new List<long>();
            foreach (var requestLabelId in request.LabelIds)
            {
                var resultId = client.CreateFavoritePage(new CreateFavoritePageContract
                {
                    Title = request.Title,
                    FavoriteLabelId = requestLabelId,
                    PageId = request.PageId
                });
                resultIds.Add(resultId);
            }

            return Json(resultIds);
        }

        public ActionResult GetPageBookmarks(long bookId)
        {
            var client = GetFavoriteClient();
            var result = client.GetPageBookmarks(bookId);
            return Json(result);
        }

        public ActionResult GetFavoriteQueries(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType,
            string filterByTitle, int start, int count)
        {
            var client = GetFavoriteClient();
            var result = client.GetFavoriteQueries(start, count, labelId, bookType, queryType, filterByTitle);
            return Json(result.List);
        }

        public ActionResult GetFavoriteQueriesCount(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType,
            string filterByTitle)
        {
            var client = GetFavoriteClient();
            var result = client.GetFavoriteQueries(0, 0, labelId, bookType, queryType, filterByTitle);
            return Json(result.TotalCount);
        }

        public ActionResult GetLabelList()
        {
            var client = GetFavoriteClient();
            var result = client.GetFavoriteLabelList();
            return Json(result);
        }

        public ActionResult GetLatestLabelList()
        {
            var client = GetFavoriteClient();
            var result = client.GetFavoriteLabelList(LatestFavoriteCount);
            return Json(result);
        }

        public ActionResult GetFavoriteList(long? labelId, FavoriteTypeEnumContract? filterByType, string filterByTitle,
            FavoriteSortEnumContract? sort, int start, int count)
        {
            if (sort == null)
                sort = FavoriteSortEnumContract.TitleAsc;

            if (filterByType != null && filterByType.Value == FavoriteTypeEnumContract.Unknown)
                filterByType = null;

            var client = GetFavoriteClient();
            var result = client.GetFavoriteItems(start, count, labelId, filterByType, filterByTitle, sort.Value);
            return Json(result.List);
        }

        public ActionResult GetFavoriteListCount(long? labelId, FavoriteTypeEnumContract? filterByType, string filterByTitle)
        {
            if (filterByType != null && filterByType.Value == FavoriteTypeEnumContract.Unknown)
                filterByType = null;

            var client = GetFavoriteClient();
            var result = client.GetFavoriteItems(0, 0, labelId, filterByType, filterByTitle, null);
            return Json(result.TotalCount);
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
            var client = GetFavoriteClient();
            var resultId = client.CreateFavoriteLabel(new FavoriteLabelContractBase
            {
                Color = request.Color,
                Name = request.Name,
            });
            return Json(resultId);
        }

        public ActionResult UpdateLabel([FromBody] UpdateLabelRequest request)
        {
            var client = GetFavoriteClient();
            client.UpdateFavoriteLabel(request.LabelId, new FavoriteLabelContractBase
            {
                Color = request.Color,
                Name = request.Name,
            });
            return Json(new { });
        }

        public ActionResult DeleteLabel([FromBody] DeleteLabelRequest request)
        {
            var client = GetFavoriteClient();
            client.DeleteFavoriteLabel(request.LabelId);
            return Json(new { });
        }

        public ActionResult UpdateFavoriteItem([FromBody] UpdateFavoriteItemRequest request)
        {
            var client = GetFavoriteClient();
            client.UpdateFavoriteItem(request.Id, new UpdateFavoriteContract
            {
                Name = request.Title,
            });
            return Json(new { });
        }

        public ActionResult DeleteFavoriteItem([FromBody] DeleteFavoriteItemRequest request)
        {
            var client = GetFavoriteClient();
            client.DeleteFavoriteItem(request.Id);
            return Json(new { });
        }
    }
}