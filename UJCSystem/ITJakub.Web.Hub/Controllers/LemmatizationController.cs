using System.Web.Mvc;
using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers
{
    public class LemmatizationController : Controller
    {
        public ActionResult Index()
        {
            return View("Lemmatization");
        }

        public ActionResult Lemmatization()
        {
            var client = new LemmatizationServiceClient();
            client.GetLemma("ahoj");

            return View("Lemmatization");
        }

        public ActionResult LemmatizationList()
        {
            return View("LemmatizationList");
        }
    }
}