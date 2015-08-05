using System.Web.Mvc;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Controllers
{
    public class HomeController : Controller
    {
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
            var client = new ItJakubServiceClient();
            client.CreateAnonymousFeedback(model.Text, model.Name, model.Email);

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
            var client = new ItJakubServiceClient();
            var result = client.GetTypeaheadAuthors(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            var client = new ItJakubServiceClient();
            var result = client.GetTypeaheadTitles(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadDictionaryHeadword(string query)
        {
            var client = new ItJakubServiceClient();
            var result = client.GetTypeaheadDictionaryHeadwords(null, null, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}