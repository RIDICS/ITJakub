using System.Web.Mvc;
using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers
{
    public class LemmatizationController : BaseController
    {
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
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadToken(query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateToken(string token, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateToken(token, description);
                return Json(resultId, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTokenCharacteristic(long tokenId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTokenCharacteristic(tokenId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.AddTokenCharacteristic(tokenId, morphologicalCharacteristic, description);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTypeaheadCanonicalForm(CanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadCanonicalForm(type, query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadHyperCanonicalForm(type, query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateCanonicalForm(tokenCharacteristicId, type, text, description);
                return Json(resultId, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.AddCanonicalForm(tokenCharacteristicId, canonicalFormId);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateHyperCanonicalForm(long canonicalFormId, HyperCanonicalFormTypeContract type, string text, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateHyperCanonicalForm(canonicalFormId, type, text, description);
                return Json(resultId, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetHyperCanonicalForm(long canonicalFormId, long hyperCanonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.SetHyperCanonicalForm(canonicalFormId, hyperCanonicalFormId);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditToken(long tokenId, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditToken(tokenId, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditTokenCharacteristic(long tokenCharacteristicId, string morphologicalCharacteristic, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditTokenCharacteristic(tokenCharacteristicId, morphologicalCharacteristic, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditCanonicalForm(long canonicalFormId, string text, CanonicalFormTypeContract type, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditCanonicalForm(canonicalFormId, text, type, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditHyperCanonicalForm(hyperCanonicalFormId, text, type, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTokenCount()
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var count = client.GetTokenCount();
                return Json(count, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTokenList(int start, int count)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var list = client.GetTokenList(start, count);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetToken(long tokenId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetToken(tokenId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var list = client.GetCanonicalFormIdList(hyperCanonicalFormId);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCanonicalFormDetail(long canonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetCanonicalFormDetail(canonicalFormId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}