using System.Net;
using System.Threading.Tasks;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public AccountController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {  
        }

        //
        // GET: /Account/Login
        [RequireHttps]
        public ActionResult Login(string returnUrl = null)
        {
            return RedirectToLocal(returnUrl);
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
        // POST: /Account/LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOut()
        {
            return new SignOutResult(new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}