using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Favorites;
using ITJakub.Web.Hub.Models.Favorite;

namespace ITJakub.Web.Hub.Controllers
{
    public class FavoriteController : BaseController
    {
        private const int AllFavoriteCount = 0;
        private const int LatestFavoriteCount = 5;

        private string CurrentUserName
        {
            get { return GetUserName(); }
        }

        public ActionResult Management()
        {
            using (var client = GetMainServiceClient())
            {
                var favoriteLabels = client.GetFavoriteLabels(AllFavoriteCount, CurrentUserName);
                var viewModel = new FavoriteManagementViewModel
                {
                    FavoriteLabels = Mapper.Map<IList<FavoriteLabelViewModel>>(favoriteLabels),
                    SortList = new List<FavoriteSortViewModel>
                    {
                        new FavoriteSortViewModel(FavoriteSortContract.TitleAsc, "Název vzestupně"),
                        new FavoriteSortViewModel(FavoriteSortContract.TitleDesc, "Název sestupně"),
                        new FavoriteSortViewModel(FavoriteSortContract.CreateTimeAsc, "Čas vytvoření vzestupně"),
                        new FavoriteSortViewModel(FavoriteSortContract.CreateTimeDesc, "Čas vytvoření sestupně")
                    },
                    FilterList = new List<FavoriteFilterViewModel>
                    {
                        new FavoriteFilterViewModel(FavoriteTypeContract.Unknown, "Vše"),
                        new FavoriteFilterViewModel(FavoriteTypeContract.Book, "Knihy"),
                        new FavoriteFilterViewModel(FavoriteTypeContract.Category, "Kategorie"),
                        new FavoriteFilterViewModel(FavoriteTypeContract.PageBookmark, "Záložky na stránky"),
                        new FavoriteFilterViewModel(FavoriteTypeContract.Query, "Vyhledávací dotazy")
                    }
                };
                return View("FavoriteManagement", viewModel);
            }
        }

        public ActionResult NewFavorite(string itemName)
        {
            if (string.IsNullOrWhiteSpace(CurrentUserName))
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
                var favoriteLabels = client.GetFavoriteLabels(AllFavoriteCount, CurrentUserName);

                var favoriteLabelViewModels = Mapper.Map<IList<FavoriteLabelViewModel>>(favoriteLabels);
                var viewModel = new NewFavoriteViewModel
                {
                    ItemName = itemName,
                    Labels = favoriteLabelViewModels
                };

                return PartialView("_NewFavorite", viewModel);
            }
        }
        
        public ActionResult Dialog()
        {
            return PartialView("Plugins/_Dialog");
        }

        public ActionResult GetFavoriteLabeledBooks(IList<long> bookIds)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabeledBooks(bookIds, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFavoriteLabeledCategories(IList<int> categoryIds)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabeledCategories(categoryIds, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabelsWithBooksAndCategories(bookType, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateFavoriteBook(long bookId, string title, IList<long> labelIds)
        {
            using (var client = GetMainServiceClient())
            {
                var resultIds = client.CreateFavoriteBook(bookId, title, labelIds, CurrentUserName);
                return Json(resultIds);
            }
        }

        public ActionResult CreateFavoriteCategory(int categoryId, string title, IList<long> labelIds)
        {
            using (var client = GetMainServiceClient())
            {
                var resultIds = client.CreateFavoriteCategory(categoryId, title, labelIds, CurrentUserName);
                return Json(resultIds);
            }
        }

        public ActionResult CreateFavoriteQuery(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string query, string title, IList<long> labelIds)
        {
            using (var client = GetMainServiceClient())
            {
                var resultIds = client.CreateFavoriteQuery(bookType, queryType, query, title, labelIds, CurrentUserName);
                return Json(resultIds);
            }
        }

        public ActionResult CreatePageBookmark(string bookXmlId, string pageXmlId, string title, long? labelId)
        {
            using (var client = GetMainServiceClient())
            {
                var resultId = client.CreatePageBookmark(bookXmlId, pageXmlId, title, labelId, CurrentUserName);
                return Json(resultId);
            }
        }

        public ActionResult GetPageBookmarks(string bookXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetPageBookmarks(bookXmlId, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFavoriteQueries(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteQueries(labelId, bookType, queryType, filterByTitle, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFavoriteQueriesCount(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteQueriesCount(labelId, bookType, queryType, filterByTitle, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLabelList()
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabels(AllFavoriteCount, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLatestLabelList()
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteLabels(LatestFavoriteCount, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
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
                var result = client.GetFavoriteItems(labelId, filterByType, filterByTitle, sort.Value, start, count, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFavoriteListCount(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle)
        {
            if (filterByType != null && filterByType.Value == FavoriteTypeContract.Unknown)
                filterByType = null;

            using (var client = GetMainServiceClient())
            {
                var result = client.GetFavoriteItemsCount(labelId, filterByType, filterByTitle, CurrentUserName);
                return Json(result, JsonRequestBehavior.AllowGet);
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

        public ActionResult CreateLabel(string name, string color)
        {
            using (var client = GetMainServiceClient())
            {
                var resultId = client.CreateFavoriteLabel(name, color, CurrentUserName);
                return Json(resultId);
            }
        }

        public ActionResult UpdateLabel(long labelId, string name, string color)
        {
            using (var client = GetMainServiceClient())
            {
                client.UpdateFavoriteLabel(labelId, name, color, CurrentUserName);
                return Json(new { });
            }
        }

        public ActionResult DeleteLabel(long labelId)
        {
            using (var client = GetMainServiceClient())
            {
                client.DeleteFavoriteLabel(labelId, CurrentUserName);
                return Json(new {});
            }
        }

        public ActionResult UpdateFavoriteItem(long id, string title)
        {
            using (var client = GetMainServiceClient())
            {
                client.UpdateFavoriteItem(id, title, CurrentUserName);
                return Json(new {});
            }
        }

        public ActionResult DeleteFavoriteItem(long id)
        {
            using (var client = GetMainServiceClient())
            {
                client.DeleteFavoriteItem(id, CurrentUserName);
                return Json(new {});
            }
        }
    }
}