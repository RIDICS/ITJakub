using System.Collections.Generic;
using System.Web.Mvc;
using ITJakub.Web.Hub.Models.Favorite;

namespace ITJakub.Web.Hub.Controllers
{
    public class FavoriteController : BaseController
    {
        private string CurrentUserName
        {
            get { return HttpContext.User.Identity.Name; }
        }

        public ActionResult NewFavorite(string itemName)
        {
            var viewModel = new NewFavoriteViewModel
            {
                ItemName = itemName,
                Labels = new List<FavoriteLabelViewModel>
                {
                    new FavoriteLabelViewModel
                    {
                        Color = "#EEB711",
                        Id = 1,
                        Name = "Výchozí - mock"
                    },
                    new FavoriteLabelViewModel
                    {
                        Color = "#10B711",
                        Id = 2,
                        Name = "Druhý - mock"
                    },
                    new FavoriteLabelViewModel
                    {
                        Color = "#EE1211",
                        Id = 3,
                        Name = "Třetí - mock"
                    },
                }
            };

            return PartialView("_NewFavorite", viewModel);
        }

        public ActionResult Dialog()
        {
            return PartialView("Plugins/_Dialog");
        }

        // TODO only for testing during development:
        public ActionResult TestForDevelop()
        {
            return View("TestForDevelop");
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

        public ActionResult CreateFavoriteBook(long bookId, string title, long? labelId)
        {
            using (var client = GetMainServiceClient())
            {
                client.CreateFavoriteBook(bookId, title, labelId, CurrentUserName);
                return Json(new {});
            }
        }

        public ActionResult CreateFavoriteCategory(int categoryId, string title, long? labelId)
        {
            using (var client = GetMainServiceClient())
            {
                client.CreateFavoriteCategory(categoryId, title, labelId, CurrentUserName);
                return Json(new {});
            }
        }

        public ActionResult GetLabelList()
        {
            var result = new List<object>();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFavoriteList(long? labelId)
        {
            var result = new List<object>();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateLabel(string name, string color)
        {
            return Json(new {});
        }

        public ActionResult UpdateLabel(long labelId, string name, string color)
        {
            return Json(new {});
        }

        public ActionResult DeleteLabel(long labelId)
        {
            return Json(new {});
        }
    }
}