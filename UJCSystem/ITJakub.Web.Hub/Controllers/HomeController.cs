using System.Web.Mvc;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;

        public HomeController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
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
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult Feedback(FeedbackViewModel model)
        {
            var name = model.Name;
            var email = model.Email;
            var text = model.Text;
            //TODO implement call to add to DB
            return View(model);
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
            var result = m_mainServiceClient.GetTypeaheadAuthors(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadTitles(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadDictionaryHeadword(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadDictionaryHeadwords(null, null, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}