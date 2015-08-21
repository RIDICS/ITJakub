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
            var resultId = m_serviceClient.CreateToken(token, description);
            return Json(resultId, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetTypeaheadCanonicalForm(CanonicalFormTypeContract type, string query)
        {
            var result = m_serviceClient.GetTypeaheadCanonicalForm(type, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadHyperCanonicalForm(string query)
        {
            var result = m_serviceClient.GetTypeaheadHyperCanonicalForm(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description)
        {
            var resultId = m_serviceClient.CreateCanonicalForm(tokenCharacteristicId, type, text, description);
            return Json(resultId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            m_serviceClient.AddCanonicalForm(tokenCharacteristicId, canonicalFormId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateHyperCanonicalForm(long canonicalFormId, HyperCanonicalFormTypeContract type, string text, string description)
        {
            var resultId = m_serviceClient.CreateHyperCanonicalForm(canonicalFormId, type, text, description);
            return Json(resultId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetHyperCanonicalForm(long canonicalFormId, long hyperCanonicalFormId)
        {
            m_serviceClient.SetHyperCanonicalForm(canonicalFormId, hyperCanonicalFormId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}