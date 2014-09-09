using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
         [AllowAnonymous]
        public ActionResult Upload()
        {
            return View();
        }

        //Dropzone upload method
        public ActionResult UploadFiles()
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                if (file == null || file.ContentLength == 0) continue;

                string pathString = Path.Combine(Server.MapPath(@""), "UploadedFiles");
                
                if (!Directory.Exists(pathString))
                    Directory.CreateDirectory(pathString);

                string path = string.Format("{0}\\{1}", pathString, file.FileName);
                file.SaveAs(path);
            }
            return Json(new {});
        }
    }
}