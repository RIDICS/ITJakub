using System.Web;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.Web.Hub.Controllers
{
    public class UploadController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();


        public ActionResult Upload()
        {
            return View();
        }


        //Dropzone upload method
        public ActionResult UploadFile()
        {
            if (Request.Files.Count == 1)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null && file.ContentLength != 0)
                {
                    ProcessedFileInfoContract fileInfo = m_serviceClient.ProcessUploadedFile(file.InputStream);
                    return Json(new {FileInfo = fileInfo});
                }
            }

            return Json(new {Error = "Some error occured in uploading file"});
        }

        public ActionResult UploadMetadata(string fileGuid, string name, string author)
        {
            m_serviceClient.SaveFileMetadata(fileGuid, name, author);
            return Json(new {});
        }

        public ActionResult UploadFrontImage(string fileGuid)
        {
            if (Request.Files.Count == 1)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null && file.ContentLength != 0)
                {
                    m_serviceClient.SaveFrontImageForFile(new UploadImageContract
                    {
                        FileGuid = fileGuid,
                        Name = file.FileName,
                        Data = file.InputStream
                    });
                    return Json(new {});
                }
            }

            return Json(new {Error = "Some error occured in uploading front image"});
        }

        public ActionResult UploadImages(string fileGuid)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (file != null && file.ContentLength != 0)
                {
                    m_serviceClient.SavePageImageForFile(new UploadImageContract
                    {
                        FileGuid = fileGuid,
                        Name = file.FileName,
                        Data = file.InputStream
                    });
                }
            }
            return Json(new {});
        }
    }
}