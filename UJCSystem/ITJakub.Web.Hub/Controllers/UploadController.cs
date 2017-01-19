using System;
using ITJakub.Shared.Contracts.Resources;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Identity;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                    using (var client = GetStreamingClient())
                    {
                        client.AddResource(
                            new UploadResourceContract
                            {
                                SessionId = request.SessionId,
                                FileName = file.FileName,
                                Data = file.OpenReadStream()
                            }
                        );
                    }
                }
            }
            return Json(new {});
        }

        [HttpPost]
        public ActionResult ProcessUploadedFiles([FromBody] ProcessUploadedFilesRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var success = client.ProcessSession(request.SessionId, request.UploadMessage);
                return Json(new {success});
            }
        }
    }
}