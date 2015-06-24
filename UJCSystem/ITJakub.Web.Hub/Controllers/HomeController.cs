using System.Web.Mvc;

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

        public ActionResult GetTypeaheadDictionaryEntry(string query)
        {
            //TODO get dictionary entries
            return Json(new [] {"TODO"}, JsonRequestBehavior.AllowGet);
        }
    }
}