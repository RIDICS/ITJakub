using System.Web.Mvc;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Areas.Tools.Controllers
{
    [RouteArea("Tools")]
    public class ToolsController : BaseController
    {
        private readonly StaticTextManager m_staticTextManager;

        public ToolsController(StaticTextManager staticTextManager)
        {
            m_staticTextManager = staticTextManager;
        }

        public ActionResult Index()
        {
            return List();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextToolsInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);

            var username = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return View(new FeedbackViewModel
                {
                    PageStaticText = pageStaticText
                });
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
            var username = HttpContext.User.Identity.Name;

            if (string.IsNullOrWhiteSpace(username))
                using (var client = GetMainServiceClient())
                {
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.Tools);
                }
            else
                using (var client = GetMainServiceClient())
                {
                    client.CreateFeedback(model.Text, username, FeedbackCategoryEnumContract.Tools);
                }

            return View("Information");
        }

        public ActionResult List()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextToolsList);
            return View("List", pageStaticText);
        }

    }
}