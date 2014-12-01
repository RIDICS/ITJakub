using System;
using System.Web.Mvc;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var user = m_serviceClient.LoginUser(); //TODO
            //if (user == null)
            //{
            //    ModelState.AddModelError("", "Invalid login attempt.");
            //    return View(model);
            //}

            //TODO save token to cookie
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [RequireHttps]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                m_serviceClient.CreateUser(model.Email, model.Password);
            }
            return View(model);
        }

        //
        // POST: /Account/LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            var httpCookie = Response.Cookies["comm_token"];
            if (httpCookie != null)
                httpCookie.Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index", "Home");
        }
    }
}