using System;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Identity;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanUploadBooks)]
    public class UploadController : BaseController
    {
        public UploadController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult Upload()
        {
            return View(new UploadViewModel {SessionId = Guid.NewGuid().ToString()});
        }

        //Dropzone upload method
        [HttpPost]
        public ActionResult UploadFile(UploadFileRequest request)
        {
            for (var i = 0; i < Request.Form.Files.Count; i++)
            {
                var file = Request.Form.Files[i];
                if (file != null && file.Length != 0)
                {
                    using (var client = GetRestClient())
                    {
                        client.UploadResource(request.SessionId, file.OpenReadStream(), file.FileName);
                    }
                }
            }
            return Json(new {});
        }

        [HttpPost]
        public ActionResult ProcessUploadedFiles([FromBody] ProcessUploadedFilesRequest request)
        {
            using (var client = GetRestClient())
            {
                client.ProcessSessionAsImport(request.SessionId, new NewBookImportContract
                {
                    Comment = request.UploadMessage,
                    ProjectId = request.ProjectId
                });
                return Json(new {success = true});
            }
        }
    }
}