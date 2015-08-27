using System;
using System.Web;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
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
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
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

        public ActionResult ProcessUploadedFiles(string sessionId, string uploadMessage)
        {
            return Json(new { success = m_serviceClient.ProcessSession(sessionId, uploadMessage) });
        }
    }
}