using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;

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


            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "Brock"));
            claims.Add(new Claim(ClaimTypes.Email, "brockallen@gmail.com"));
            var id = new ClaimsIdentity(claims,
                                        DefaultAuthenticationTypes.ApplicationCookie);

            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(id);
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
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}