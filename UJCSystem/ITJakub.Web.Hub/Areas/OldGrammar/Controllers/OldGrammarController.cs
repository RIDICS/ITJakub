using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Areas.OldGrammar.Controllers
{
    [RouteArea("OldGrammar")]
    public class OldGrammarController : Controller
    {

        private readonly ItJakubServiceClient m_mainServiceClient = new ItJakubServiceClient();
        private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
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

            var user = m_mainServiceEncryptedClient.FindUserByUserName(username);
            var viewModel = new FeedbackViewModel
            {
                Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                Email = user.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            var username = HttpContext.User.Identity.Name;

            if (string.IsNullOrWhiteSpace(username))
                m_mainServiceClient.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.OldGrammar);
            else
                m_mainServiceEncryptedClient.CreateFeedback(model.Text, username, FeedbackCategoryEnumContract.OldGrammar);

            return View("Information");
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadAuthorsByBookType(query, BookTypeEnumContract.Grammar);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.Grammar, null, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}