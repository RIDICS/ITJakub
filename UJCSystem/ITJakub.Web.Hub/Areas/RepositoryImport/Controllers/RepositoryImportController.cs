using System.Linq;
using ITJakub.Web.Hub.Areas.RepositoryImport.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class RepositoryImportController : BaseController
    {
        public RepositoryImportController(CommunicationProvider communicationProvider, HttpErrorCodeTranslator httpErrorCodeTranslator) : base(
            communicationProvider, httpErrorCodeTranslator)
        {
        }

        public IActionResult List()
        {
            using (var client = GetRestClient())
            {
                var status = client.GetImportStatus();

                if (status == null || status.All(x => x.IsCompleted))
                {
                    var externalRepositories = client.GetAllExternalRepositories();
                    var list = externalRepositories.Select(x => new CheckBoxEntity (x.Id,x.Name)).ToList();
                    return View(new ImportViewModel { ExternalRepositoryCheckBoxes = list });
                }

                return View("ImportStatus",
                    new ImportViewModel
                    {
                        ExternalRepositoryCheckBoxes = status.Select(x => new CheckBoxEntity(x.ExternalRepositoryId, x.ExternalRepositoryName)).ToList()
                    });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartImport(ImportViewModel model)
        {
            using (var client = GetRestClient())
            {
                client.StartImport(model.ExternalRepositoryCheckBoxes.Where(x => x.IsChecked).Select(x => x.Id).ToList());
                return View("ImportStatus",
                    new ImportViewModel
                        {ExternalRepositoryCheckBoxes = model.ExternalRepositoryCheckBoxes.Where(x => x.IsChecked).ToList()});
            }
        }

        public IActionResult GetImportStatus()
        {
            using (var client = GetRestClient())
            {
                var status = client.GetImportStatus();
                return Json(status);
            }
        }
        
        public void CancelImport(int id)
        {
            using (var client = GetRestClient())
            {
                client.CancelImportTask(id);
            }
        }
    }
}