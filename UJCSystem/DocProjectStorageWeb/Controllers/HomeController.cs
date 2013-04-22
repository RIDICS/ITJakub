using DocProjectStorageWeb;
using DocProjectStorageWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using DocProjectStorageWeb.WebModels;

namespace DocProjectStorageWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly DocProjectModelContainer m_container = new DocProjectModelContainer();
        // private string userName;

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return View("Login");

            string appdatafolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            string[] lines = System.IO.File.ReadAllLines(Server.MapPath("App_Data/Motto.txt"));

            Random random = new Random();
            int position = random.Next(lines.Length);
            ViewBag.Message = lines[position];
            var selectedDocuments = from cs in m_container.DocProjectEntities
                                    select cs;


            DashboardModel model = new DashboardModel();

            //DocumentProject firstDocument = selectedDocuments.FirstOrDefault<DocumentProject>();
            List<DocProjectEntity> documentList = selectedDocuments.ToList<DocProjectEntity>();
            model.Projects = documentList;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid && WebSecurity.Login(model.Email, model.Password, model.RememberMe))
            {
                return RedirectToAction("Index", "Home");
            }
            /*else
            {
                return View("Login", model);
            }*/

            return View("Login");
        }

        public ActionResult Admin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Home");
        }

    }
}
