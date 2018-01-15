using System.Net;
using System.Threading.Tasks;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using AuthenticationManager = ITJakub.Web.Hub.Core.Managers.AuthenticationManager;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly AuthenticationManager m_authenticationManager;

        public AccountController(CommunicationProvider communicationProvider, AuthenticationManager authenticationManager) : base(communicationProvider)
        {
            m_authenticationManager = authenticationManager;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        //[RequireHttps]
        public ActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        //[RequireHttps]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await m_authenticationManager.SignInAsync(model);

                return RedirectToLocal(returnUrl);
            }
            catch (HttpErrorCodeException exception)
            {
                switch (exception.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        ModelState.AddModelError("", "Přihlášení se nezdařilo.");
                        break;
                    default:
                        ModelState.AddModelError("", exception.Message);
                        break;
                }

                return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        //[RequireHttps]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[RequireHttps]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new CreateUserContract
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    NewPassword = model.Password,
                };

                try
                {
                    using (var client = GetRestClient())
                    {
                        client.CreateNewUser(user);
                    }

                    await m_authenticationManager.SignInAsync(new LoginViewModel
                    {
                        UserName = model.UserName,
                        Password = model.Password,
                    });

                    return RedirectToLocal("");
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOut()
        {
            await m_authenticationManager.SignOutAsync();
            
            return RedirectToLocal("");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private void AddErrors(HttpErrorCodeException exception)
        {
            if (exception.ValidationErrors == null)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return;
            }
            foreach (var error in exception.ValidationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
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