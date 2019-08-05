using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.Shared.Const;
using ITJakub.Web.Hub.Options;

namespace ITJakub.Web.Hub.Areas.Derivation.Controllers
{
    [LimitedAccess(PortalType.ResearchPortal)]
    [Area("Derivation")]
    public class DerivationController : BaseController
    {
        public DerivationController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        [Authorize(VokabularPermissionNames.DerivateLemmatization)]
        public ActionResult Index()
        {
            return View("Derivation");
        }
        
        [Authorize(VokabularPermissionNames.DerivateLemmatization)]
        public ActionResult Derivation()
        {
            return View("Derivation");
        }
        
        [Authorize(VokabularPermissionNames.DerivateLemmatization)]
        public ActionResult GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var result = client.GetTypeaheadHyperCanonicalForm(type, query);
                return Json(result);
            }
        }

        [Authorize(VokabularPermissionNames.DerivateLemmatization)]
        public ActionResult GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            using (var client = GetLemmationzationServiceClient())
            {
                var list = client.GetCanonicalFormIdList(hyperCanonicalFormId);
                return Json(list);
            }
        }

        [Authorize(VokabularPermissionNames.DerivateLemmatization)]
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