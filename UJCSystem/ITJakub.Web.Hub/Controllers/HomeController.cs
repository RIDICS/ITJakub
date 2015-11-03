using System.Web;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNet.Identity.Owin;

namespace ITJakub.Web.Hub.Controllers
{
    public class HomeController : BaseController
    {        
        private ApplicationUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contacts()
        {
            return View();
        }

        public ActionResult Copyright()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return View();
            }

            using (var client = GetEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new FeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
           
                using (var client = GetMainServiceClient())
                {
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.None);
                }

          


            return View("Index");
        }

        public ActionResult HowToCite()
        {
            return View();
        }

        public ActionResult Links()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            using (var client = GetMainServiceClient()) { 
            var result = client.GetTypeaheadAuthors(query);
            return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            using (var client = GetMainServiceClient()) { 
                var result = client.GetTypeaheadTitles(query);
            return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTypeaheadDictionaryHeadword(string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadDictionaryHeadwords(null, null, query, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
}