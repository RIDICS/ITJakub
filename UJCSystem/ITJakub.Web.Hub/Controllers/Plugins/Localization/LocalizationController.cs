using ITJakub.Web.Hub.Core.Communication;
using Localization.AspNetCore.Service;
using Localization.CoreLibrary.Util;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITJakub.Web.Hub.Controllers.Plugins.Localization
{
    [Route("features/[controller]")]
    public class LocalizationController : BaseController
    {
        private readonly IDictionary m_dictionary;

        public LocalizationController(CommunicationProvider communicationProvider, IDictionary dictionary) : base(communicationProvider)
        {
            m_dictionary = dictionary;
        }

        [HttpGet("{scope}")]
        public ActionResult Get(string scope)
        {
            return Json("{}");
            //return Json(m_dictionary.GetDictionary(scope, LocTranslationSource.Auto));
        }    
    }
}
