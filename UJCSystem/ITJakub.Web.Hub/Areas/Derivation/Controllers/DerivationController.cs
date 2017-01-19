using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.Derivation.Controllers
{
    [Area("Derivation")]
    public class DerivationController : BaseController
    {
        public DerivationController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        [Authorize(Roles = CustomRole.CanDerivateLemmatization)]
        public ActionResult Index()
        {
            return View("Derivation");
        }
        
        [Authorize(Roles = CustomRole.CanDerivateLemmatization)]
        public ActionResult Derivation()
        {
            return View("Derivation");
        }
        
        [Authorize(Roles = CustomRole.CanDerivateLemmatization)]
        public ActionResult GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadHyperCanonicalForm(type, query);
                return Json(result);
            }
        }

        [Authorize(Roles = CustomRole.CanDerivateLemmatization)]
        public ActionResult GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var list = client.GetCanonicalFormIdList(hyperCanonicalFormId);
                return Json(list);
            }
        }

        [Authorize(Roles = CustomRole.CanDerivateLemmatization)]
        public ActionResult GetCanonicalFormDetail(long canonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetCanonicalFormDetail(canonicalFormId);
                return Json(result);
            }
        }
    }
}