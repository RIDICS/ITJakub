using System;
using System.Web;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;
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
        [RequireHttps]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            LoginUserResultContract result = m_serviceClient.LoginUser(new LoginUserContract
            {
                AuthenticationProvider = AuthProviderEnumContract.ItJakub,
                Email = model.Email,
                Password = model.Password
            });

            if (result == null || !result.Successfull)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            //TODO save token to cookie
            ModelState.AddModelError("", "Login Successfull");
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [RequireHttps]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                m_serviceClient.CreateUser(new CreateUserContract
                {
                    AuthenticationProvider = AuthProviderEnumContract.ItJakub,
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                });
            }
            return View("Login");
        }

        //
        // POST: /Account/LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {

            //HttpCookie httpCookie = Response.Cookies["comm_token"];
            //if (httpCookie != null)
            //    httpCookie.Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index", "Home");
        }
    }
}