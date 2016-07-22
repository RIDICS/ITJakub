using System.Web;
using System.Web.Mvc;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNet.Identity.Owin;

namespace ITJakub.Web.Hub.Controllers
{
    public class HomeController : BaseController
    {
        private readonly StaticTextManager m_staticTextManager;

        public HomeController(StaticTextManager staticTextManager)
        {
            m_staticTextManager = staticTextManager;
        }

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
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeAbout);
            return View(staticTextViewModel);
        }

        public ActionResult Contacts()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeContacts);
            return View(staticTextViewModel);
        }

        public ActionResult Copyright()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeCopyright);
            return View(staticTextViewModel);
        }

        public ActionResult Feedback()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);

            var username = User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                var viewModel = new FeedbackViewModel
                {
                    PageStaticText = pageStaticText
                };

                return View(viewModel);
            }

            using (var client = GetEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new FeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email,
                    PageStaticText = pageStaticText
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
                if (Request.IsAuthenticated)
                {
                    client.CreateFeedback(model.Text, GetUserName(), FeedbackCategoryEnumContract.None);
                }
                else
                {
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.None);
                }
            }

            return View("Index");
        }

        public ActionResult HowToCite()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeHowToCite);
            return View(staticTextViewModel);
        }

        public ActionResult Links()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeLinks);
            return View(staticTextViewModel);
        }

        public ActionResult Support()
        {
            var staticTextViewModel = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeSupport);
            return View(staticTextViewModel);
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