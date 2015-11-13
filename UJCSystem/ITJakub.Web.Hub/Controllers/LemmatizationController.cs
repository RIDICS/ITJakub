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

        public ActionResult Derivation()
        {
            return View("Derivation");
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

        public ActionResult GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            var result = m_serviceClient.GetTypeaheadHyperCanonicalForm(type, query);
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

        public ActionResult EditToken(long tokenId, string description)
        {
            m_serviceClient.EditToken(tokenId, description);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditTokenCharacteristic(long tokenCharacteristicId, string morphologicalCharacteristic, string description)
        {
            m_serviceClient.EditTokenCharacteristic(tokenCharacteristicId, morphologicalCharacteristic, description);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditCanonicalForm(long canonicalFormId, string text, CanonicalFormTypeContract type, string description)
        {
            m_serviceClient.EditCanonicalForm(canonicalFormId, text, type, description);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description)
        {
            m_serviceClient.EditHyperCanonicalForm(hyperCanonicalFormId, text, type, description);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTokenCount()
        {
            var count = m_serviceClient.GetTokenCount();
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTokenList(int start, int count)
        {
            var list = m_serviceClient.GetTokenList(start, count);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToken(long tokenId)
        {
            var result = m_serviceClient.GetToken(tokenId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            var list = m_serviceClient.GetCanonicalFormIdList(hyperCanonicalFormId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCanonicalFormDetail(long canonicalFormId)
        {
            var result = m_serviceClient.GetCanonicalFormDetail(canonicalFormId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTokenCharacteristic(long tokenCharacteristicId)
        {
            m_serviceClient.DeleteTokenCharacteristic(tokenCharacteristicId);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            m_serviceClient.RemoveCanonicalForm(tokenCharacteristicId, canonicalFormId);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveHyperCanonicalForm(long canonicalFormId)
        {
            m_serviceClient.RemoveHyperCanonicalForm(canonicalFormId);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }
    }
}