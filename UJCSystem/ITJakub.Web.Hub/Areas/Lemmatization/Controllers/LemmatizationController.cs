using System.Web.Mvc;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Identity;

namespace ITJakub.Web.Hub.Areas.Lemmatization.Controllers
{
    public class LemmatizationController : BaseController
    {
        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult Index()
        {
            return View("Lemmatization");
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult Lemmatization()
        {
            return View("Lemmatization");
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization)]
        public ActionResult List()
        {
            return View("List");
        }
        
        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTypeaheadToken(string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadToken(query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult CreateToken(string token, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateToken(token, description);
                return Json(resultId, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTokenCharacteristic(long tokenId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTokenCharacteristic(tokenId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.AddTokenCharacteristic(tokenId, morphologicalCharacteristic, description);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTypeaheadCanonicalForm(CanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadCanonicalForm(type, query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadHyperCanonicalForm(type, query);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateCanonicalForm(tokenCharacteristicId, type, text, description);
                return Json(resultId, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.AddCanonicalForm(tokenCharacteristicId, canonicalFormId);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult CreateHyperCanonicalForm(long canonicalFormId, HyperCanonicalFormTypeContract type, string text, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateHyperCanonicalForm(canonicalFormId, type, text, description);
                return Json(resultId, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult SetHyperCanonicalForm(long canonicalFormId, long hyperCanonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.SetHyperCanonicalForm(canonicalFormId, hyperCanonicalFormId);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditToken(long tokenId, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditToken(tokenId, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditTokenCharacteristic(long tokenCharacteristicId, string morphologicalCharacteristic, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditTokenCharacteristic(tokenCharacteristicId, morphologicalCharacteristic, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditCanonicalForm(long canonicalFormId, string text, CanonicalFormTypeContract type, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditCanonicalForm(canonicalFormId, text, type, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditHyperCanonicalForm(hyperCanonicalFormId, text, type, description);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTokenCount()
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var count = client.GetTokenCount();
                return Json(count, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTokenList(int start, int count)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var list = client.GetTokenList(start, count);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization+","+CustomRole.CanEditLemmatization)]
        public ActionResult GetToken(long tokenId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetToken(tokenId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization + "," +CustomRole.CanDerivateLemmatization)]
        public ActionResult GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var list = client.GetCanonicalFormIdList(hyperCanonicalFormId);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization + "," + CustomRole.CanDerivateLemmatization)]
        public ActionResult GetCanonicalFormDetail(long canonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetCanonicalFormDetail(canonicalFormId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult DeleteTokenCharacteristic(long tokenCharacteristicId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.DeleteTokenCharacteristic(tokenCharacteristicId);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult RemoveCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.RemoveCanonicalForm(tokenCharacteristicId, canonicalFormId);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult RemoveHyperCanonicalForm(long canonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.RemoveHyperCanonicalForm(canonicalFormId);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}