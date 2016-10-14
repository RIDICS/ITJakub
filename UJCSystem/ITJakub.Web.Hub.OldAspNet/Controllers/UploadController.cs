using System;
using System.Web.Mvc;
using ITJakub.Shared.Contracts.Resources;
using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanUploadBooks)]
    public class UploadController : BaseController
    {
        public ActionResult Upload()
        {
            return View(new UploadViewModel {SessionId = Guid.NewGuid().ToString()});
        }

        //Dropzone upload method
        public ActionResult UploadFile(string sessionId)
        {
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null && file.ContentLength != 0)
                {
                    using (var client = GetStreamingClient())
                    {
                        client.AddResource(
                            new UploadResourceContract
                            {
                                SessionId = sessionId,
                                FileName = file.FileName,
                                Data = file.InputStream
                            }
                            );
                    }
                }
            }
            return Json(new {});
        }

        public ActionResult ProcessUploadedFiles(ProcessUploadedFilesRequest request)
        {
            using (var client = GetMainServiceClient())
            {
                var success = client.ProcessSession(request.SessionId, request.UploadMessage);
                return Json(new {success});
            }
        }
    }
}