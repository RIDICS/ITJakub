using System.Web;
using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class UploadController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient=new ItJakubServiceClient();
        // GET: Upload
         [AllowAnonymous]
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
                    var fileInfo=m_serviceClient.ProcessUploadedFile(file.InputStream);
                    return Json(new {FileInfo = fileInfo});
                }
            }

            return Json(new {Error="Some error occured in uploading file"});
        }

        public ActionResult UploadFrontImage()
        {
            if (Request.Files.Count == 1)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null && file.ContentLength != 0)
                {
                    m_serviceClient.SaveFrontImageForFile(file.InputStream);
                    return Json(new {});
                }
            }

            return Json(new { Error = "Some error occured in uploading front image" });
        }

        public ActionResult UploadImages()
        {

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (file != null && file.ContentLength != 0)
                {
                    m_serviceClient.SaveImagesForFile(file.InputStream);
                    return Json(new { });
                }
            }

            return Json(new { Error = "Some error occured in uploading other images" });
        }
    }
}