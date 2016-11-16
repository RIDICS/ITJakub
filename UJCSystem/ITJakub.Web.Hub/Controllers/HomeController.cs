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
        private readonly FeedbacksManager m_feedbacksManager;

        public HomeController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        private ApplicationUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = string.Empty, Controller = "Home"};
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
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, GetEncryptedClient(), GetUserName());
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                m_feedbacksManager.FillViewModel(model, StaticTexts.TextHomeFeedback, GetFeedbackFormIdentification());
                return View(model);
            }

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.None, GetMainServiceClient(), Request.IsAuthenticated, GetUserName());
            return View("Feedback/FeedbackSuccess");
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