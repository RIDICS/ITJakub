using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Models;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectImportSessionController : BaseController
    {
        private readonly ProjectResourceManager m_resourceManager;

        public ProjectImportSessionController(ProjectResourceManager resourceManager)
        {
            m_resourceManager = resourceManager;
        }

        [Authorize(VokabularPermissionNames.UploadBook)]
        [HttpPost("{sessionId}/resource")]
        public void UploadResource(string sessionId, [FromForm] FormFileContract formData)
        {
            using (var fileStream = formData.File.OpenReadStream())
            {
                m_resourceManager.UploadResource(sessionId, fileStream, formData.File.FileName);
            }
        }

        [Authorize(VokabularPermissionNames.UploadBook)]
        [HttpPost("{sessionId}")]
        public void ProcessSessionAsImport(string sessionId, [FromBody] NewBookImportContract info)
        {
            m_resourceManager.ProcessSessionAsImport(sessionId, info.ProjectId, info.Comment);
        }
    }
}