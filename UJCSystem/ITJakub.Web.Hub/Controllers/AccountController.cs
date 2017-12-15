using System.Threading.Tasks;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

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

            //var result =
            //    await m_signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            await m_authenticationManager.SignInAsync(model);
            // TODO handling errors

            return RedirectToLocal(returnUrl);

            //if (result.Succeeded)
            //{
            //    return RedirectToLocal(returnUrl);
            //}
            //else
            //{
            //    ModelState.AddModelError("", "Přihlášení se nezdařilo.");
            //    return View(model);
            //}
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
            // TODO add data validation
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

                using (var client = GetRestClient())
                {
                    client.CreateNewUser(user);
                }

                await m_authenticationManager.SignInAsync(new LoginViewModel
                {
                    UserName = model.UserName,
                    Password = model.Password,
                });

                //var result = await m_userManager.CreateAsync(user, model.Password);
                //if (result.Succeeded)
                //{
                //    await m_signInManager.SignInAsync(user, false);
                //    return RedirectToLocal("");
                //}
                //AddErrors(result);
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", string.Format("{0}: {1}", error.Code, error.Description));
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