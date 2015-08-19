using System.Web.Mvc;
using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers
{
    public class LemmatizationController : Controller
    {
        private readonly LemmatizationServiceClient m_serviceClient;

        public LemmatizationController()
        {
            m_serviceClient = new LemmatizationServiceClient();
        }

        public ActionResult Index()
        {
            return View("Lemmatization");
        }

        public ActionResult Lemmatization()
        {
            return View("Lemmatization");
        }

        public ActionResult LemmatizationList()
        {
            return View("LemmatizationList");
        }

        public ActionResult GetTypeaheadToken(string query)
        {
            var result = m_serviceClient.GetTypeaheadToken(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateToken(string token, string description)
        {
            var result = m_serviceClient.CreateToken(token, description);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTokenCharacteristic(long tokenId)
        {
            var result = m_serviceClient.GetTokenCharacteristic(tokenId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description)
        {
            var result = m_serviceClient.AddTokenCharacteristic(tokenId, morphologicalCharacteristic, description);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}