using System.Collections.Generic;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class ExternalRepositoryController : BaseController
    { 
        private const int RepositoryCount = 5;

        public ExternalRepositoryController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
          
        }


        public IActionResult ExternalRepositoryList()
        {
            using (var client = GetRestClient())
            {
                var externalRepositories = client.GetExternalRepositoryList(0, RepositoryCount, true);
                return View("ExternalRepositoryList", externalRepositories);
            }
        }
    }
}