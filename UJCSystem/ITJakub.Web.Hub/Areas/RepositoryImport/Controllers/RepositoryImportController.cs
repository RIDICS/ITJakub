using System.Linq;
using ITJakub.Web.Hub.Areas.RepositoryImport.Models;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts;

namespace ITJakub.Web.Hub.Areas.RepositoryImport.Controllers
{
    [RequireHttps]
    [Authorize]
    [Area("RepositoryImport")]
    public class RepositoryImportController : BaseController
    {
        private readonly IMainServiceClientLocalization m_mainServiceLocalization;

        public RepositoryImportController(CommunicationProvider communicationProvider, IMainServiceClientLocalization serviceClientLocalization) : base(communicationProvider)
        {
            m_mainServiceLocalization = serviceClientLocalization;
        }

        public IActionResult List()
        {
            var client = GetExternalRepositoryClient();
            var status = client.GetImportStatus();

            if (status == null || status.All(x => x.IsCompleted))
            {
                var externalRepositories = client.GetAllExternalRepositories();
                var list = externalRepositories.Select(x => new CheckBoxEntity(x.Id, x.Name)).ToList();
                return View(new ImportViewModel {ExternalRepositoryCheckBoxes = list});
            }

            return View("ImportStatus",
                new ImportViewModel
                {
                    ExternalRepositoryCheckBoxes = status.Select(x => new CheckBoxEntity(x.ExternalRepositoryId, x.ExternalRepositoryName))
                        .ToList()
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartImport(ImportViewModel model)
        {
            var client = GetExternalRepositoryClient();
            client.StartImport(model.ExternalRepositoryCheckBoxes.Where(x => x.IsChecked).Select(x => x.Id).ToList());
            return View("ImportStatus",
                new ImportViewModel
                    {ExternalRepositoryCheckBoxes = model.ExternalRepositoryCheckBoxes.Where(x => x.IsChecked).ToList()});
        }

        public IActionResult GetImportStatus()
        {
            var client = GetExternalRepositoryClient();
            var status = client.GetImportStatus();
            foreach (var statusItem in status)
            {
                if (m_mainServiceLocalization.TryLocalizeErrorCode(statusItem.FaultedMessage, out var localizedString,
                    statusItem.FaultedMessageParams))
                {
                    statusItem.FaultedMessage = localizedString;
                }
            }
            return Json(status);
        }

        public void CancelImport(int id)
        {
            var client = GetExternalRepositoryClient();
            client.CancelImportTask(id);
        }
    }
}