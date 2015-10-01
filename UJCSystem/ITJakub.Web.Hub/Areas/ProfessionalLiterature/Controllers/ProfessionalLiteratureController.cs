using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Areas.ProfessionalLiterature.Controllers
{
    [RouteArea("ProfessionalLiterature")]
    public class ProfessionalLiteratureController : BaseController
    {        
        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }
        
        public ActionResult List()
        {
            return View();
        }


        public ActionResult Feedback()
        {
            var username = HttpContext.User.Identity.Name;
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
            var username = HttpContext.User.Identity.Name;

            if (string.IsNullOrWhiteSpace(username))
                using (var client = GetAuthenticatedClient())
                {
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.ProfessionalLiterature);
                }
            else
                using (var client = GetAuthenticatedClient())
                {
                    client.CreateFeedback(model.Text, username, FeedbackCategoryEnumContract.ProfessionalLiterature);
                }

            return View("Information");
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            using (var client = GetAuthenticatedClient())
            {
                var result = client.GetTypeaheadAuthorsByBookType(query, BookTypeEnumContract.ProfessionalLiterature);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            using (var client = GetAuthenticatedClient())
            {
                var result = client.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.ProfessionalLiterature, null, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        }
    }
}