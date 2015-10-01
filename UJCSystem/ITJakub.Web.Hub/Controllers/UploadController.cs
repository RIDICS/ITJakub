using System;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts.Resources;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Controllers
{
    //[Authorize]
    public class UploadController : BaseController
    {
        //private readonly ItJakubServiceClient m_serviceClient = GetUnsecuredClient();

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

        public ActionResult ProcessUploadedFiles(string sessionId, string uploadMessage)
        {
            using (var client = GetUnsecuredClient())
            {
                var success = client.ProcessSession(sessionId, uploadMessage);
                return Json(new {success});
            }
        }
    }
}