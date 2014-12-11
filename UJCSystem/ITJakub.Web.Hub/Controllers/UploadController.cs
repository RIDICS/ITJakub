using System;
using System.Web.Mvc;
using ITJakub.Shared.Contracts.Resources;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Controllers
{
    //[Authorize]
    public class UploadController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

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
                    m_serviceClient.AddResource(
                        new UploadResourceContract
                        {
                            SessionId = sessionId,
                            FileName = file.FileName,
                            Data = file.InputStream
                        }
                        );
                }
            }
            return Json(new {});
        }

        public ActionResult ProcessUploadedFiles(string sessionId)
        {
            //TODO process here 
            return Json(new {success = true});
        }
    }
}