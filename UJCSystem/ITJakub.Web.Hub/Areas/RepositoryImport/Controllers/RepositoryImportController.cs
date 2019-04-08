using System.Linq;
using ITJakub.Web.Hub.Areas.RepositoryImport.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class RepositoryImportController : BaseController
    {
        public RepositoryImportController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public IActionResult List()
        {
            using (var client = GetRestClient())
            {
                var externalRepositories = client.GetAllExternalRepositories();
                var list = externalRepositories.Select(x => new ExternalRepositoryCheckBox {Id = x.Id, Name = x.Name, IsChecked = false}).ToList();
                return View(new ImportViewModel {ExternalRepositoryCheckBoxes = list});
            }
        }

        [HttpPost]
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