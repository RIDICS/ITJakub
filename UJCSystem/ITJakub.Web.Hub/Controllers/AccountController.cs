using System;
using System.Threading.Tasks;
using AutoMapper;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.AspNetCore.Extensions;

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
            try
            {
                using (var client = GetRestClient())
                {
                    client.CreateUserIfNotExist(HttpContext.User.GetId().GetValueOrDefault());
                }

                return RedirectToLocal("");
            }
            catch (HttpErrorCodeException e)
            {
                AddErrors(e);
            }

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
        // POST: /Account/Register
        [HttpPost]
        [RequireHttps]
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
        // GET: /Account/AccountSettings
        public IActionResult AccountSettings()
        {
            using (var client = GetRestClient())
            {
                var user = client.GetCurrentUser();
                var viewmodel = Mapper.Map<AccountDetailViewModel>(user);
                viewmodel.UpdateAccountViewModel = Mapper.Map<UpdateAccountViewModel>(user);
                viewmodel.UpdatePasswordViewModel = null;
                return View(viewmodel);
            }
        }

        //
        // POST: /Account/UpdateAccount
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAccount(UpdateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = GetRestClient())
                    {
                        UpdateUserContract updateUserContract = new UpdateUserContract
                        {
                            AvatarUrl = null, //TODO
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName
                        };

                        client.UpdateCurrentUser(updateUserContract);
                        return Ok();
                    }
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
            }

            var viewModel = new AccountDetailViewModel {UpdateAccountViewModel = model};
            return View("AccountSettings", viewModel);
        }

        //
        // POST: /Account/UpdatePassword
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(UpdatePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = GetRestClient())
                    {
                        UpdateUserPasswordContract updateUserPasswordContract = new UpdateUserPasswordContract
                        {
                            NewPassword = model.Password,
                            OldPassword = model.OldPassword
                        };

                        client.UpdateCurrentPassword(updateUserPasswordContract);
                        return Ok();
                    }
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
            }

            var viewModel = new AccountDetailViewModel { UpdatePasswordViewModel = model };
            return View("AccountSettings", viewModel);
        }

        //
        // POST: /Account/LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOut()
        {
            return new SignOutResult(new[] {CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme});
        }


        //
        // POST: /Account/ClientLogOut
        [HttpPost]
        public IActionResult ClientLogOut()
        {
            return new SignOutResult(new[] {CookieAuthenticationDefaults.AuthenticationScheme});
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