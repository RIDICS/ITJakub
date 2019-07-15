using AutoMapper;
using ITJakub.Web.Hub.Constants;
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
            return RedirectToLocal("");
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
        public ActionResult Register(RegisterViewModel model)
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

                    return RedirectToAction(nameof(SuccessfulRegistration));
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SuccessfulRegistration()
        {
            return View();
        }

        //
        // GET: /Account/AccountSettings
        public IActionResult AccountSettings()
        {
            using (var client = GetRestClient())
            {
                var user = client.GetCurrentUser();
                var viewmodel = new AccountDetailViewModel
                {
                    UpdateUserViewModel = Mapper.Map<UpdateUserViewModel>(user),
                    UpdatePasswordViewModel = null,
                    UpdateContactViewModel = Mapper.Map<UpdateContactViewModel>(user),
                    ActualTab = AccountTab.UpdateAccount
                };
                return View(viewmodel);
            }
        }

        //
        // POST: /Account/UpdateAccount
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAccount(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = GetRestClient())
                    {
                        var updateUserContract = new UpdateUserContract
                        {
                            AvatarUrl = null, //TODO Avatar
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName
                        };

                        client.UpdateCurrentUser(updateUserContract);
                        ViewData.Add(AccountConstants.SuccessUserUpdate, true);
                    }
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
            }


            using (var client = GetRestClient())
            {
                var user = client.GetCurrentUser();
                var updateUserViewModel = Mapper.Map<UpdateUserViewModel>(user);
                var viewModel = new AccountDetailViewModel
                {
                    UpdateUserViewModel = updateUserViewModel,
                    UpdatePasswordViewModel = null
                };
                return View("AccountSettings", viewModel);
            }
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
                        var updateUserPasswordContract = new UpdateUserPasswordContract
                        {
                            NewPassword = model.Password,
                            OldPassword = model.OldPassword
                        };

                        client.UpdateCurrentPassword(updateUserPasswordContract);
                        ViewData.Add(AccountConstants.SuccessPasswordUpdate, true);
                    }
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
            }

            using (var client = GetRestClient())
            {
                var user = client.GetCurrentUser();
                var viewModel = new AccountDetailViewModel
                {
                    UpdateUserViewModel = Mapper.Map<UpdateUserViewModel>(user),
                    UpdatePasswordViewModel = model,
                    UpdateContactViewModel = Mapper.Map<UpdateContactViewModel>(user),
                    ActualTab = AccountTab.UpdatePassword
                };
                return View("AccountSettings", viewModel);
            }
        }

        //
        // POST: /Account/UpdateContact
        [HttpPost]
        public IActionResult UpdateContact([FromBody] UpdateUserContactContract updateUserContactContract)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    client.UpdateCurrentUserContact(updateUserContactContract);
                }
            }
            catch (HttpErrorCodeException e)
            {
                AddErrors(e);
                return BadRequest(ModelState);
            }

            return Ok();
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