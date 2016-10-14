using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models.Requests.Lemmatization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.Lemmatization.Controllers
{
    [Area("Lemmatization")]
    public class LemmatizationController : BaseController
    {
        public LemmatizationController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

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

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
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
                return Json(result);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult CreateToken([FromBody] CreateTokenRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateToken(request.Token, request.Description);
                return Json(resultId);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTokenCharacteristic(long tokenId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTokenCharacteristic(tokenId);
                return Json(result);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult AddTokenCharacteristic([FromBody] AddTokenCharacteristicRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.AddTokenCharacteristic(request.TokenId, request.MorphologicalCharacteristic, request.Description);
                return Json(result);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTypeaheadCanonicalForm(CanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadCanonicalForm(type, query);
                return Json(result);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadHyperCanonicalForm(type, query);
                return Json(result);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult CreateCanonicalForm([FromBody] CreateCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateCanonicalForm(request.TokenCharacteristicId, request.Type, request.Text, request.Description);
                return Json(resultId);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult AddCanonicalForm([FromBody] AddCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.AddCanonicalForm(request.TokenCharacteristicId, request.CanonicalFormId);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult CreateHyperCanonicalForm([FromBody] CreateHyperCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var resultId = client.CreateHyperCanonicalForm(request.CanonicalFormId, request.Type, request.Text, request.Description);
                return Json(resultId);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult SetHyperCanonicalForm([FromBody] SetHyperCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.SetHyperCanonicalForm(request.CanonicalFormId, request.HyperCanonicalFormId);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditToken([FromBody] EditTokenRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditToken(request.TokenId, request.Description);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditTokenCharacteristic([FromBody] EditTokenCharacteristicRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditTokenCharacteristic(request.TokenCharacteristicId, request.MorphologicalCharacteristic, request.Description);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditCanonicalForm([FromBody] EditCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditCanonicalForm(request.CanonicalFormId, request.Text, request.Type, request.Description);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult EditHyperCanonicalForm([FromBody] EditHyperCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.EditHyperCanonicalForm(request.HyperCanonicalFormId, request.Text, request.Type, request.Description);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTokenCount()
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var count = client.GetTokenCount();
                return Json(count);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetTokenList(int start, int count)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var list = client.GetTokenList(start, count);
                return Json(list);
            }
        }

        [Authorize(Roles = CustomRole.CanReadLemmatization + "," + CustomRole.CanEditLemmatization)]
        public ActionResult GetToken(long tokenId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetToken(tokenId);
                return Json(result);
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult DeleteTokenCharacteristic([FromBody] DeleteTokenCharacteristicRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.DeleteTokenCharacteristic(request.TokenCharacteristicId);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult RemoveCanonicalForm([FromBody] RemoveCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.RemoveCanonicalForm(request.TokenCharacteristicId, request.CanonicalFormId);
                return Json(new { });
            }
        }

        [Authorize(Roles = CustomRole.CanEditLemmatization)]
        public ActionResult RemoveHyperCanonicalForm([FromBody] RemoveHyperCanonicalFormRequest request)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                client.RemoveHyperCanonicalForm(request.CanonicalFormId);
                return Json(new { });
            }
        }
    }
}