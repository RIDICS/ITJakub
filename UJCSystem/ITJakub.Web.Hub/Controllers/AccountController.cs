using System.Net;
using AutoMapper;
using ITJakub.Web.Hub.Constants;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models.Requests.Permission;
using ITJakub.Web.Hub.Models.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly ILocalizationService m_localizationService;

        public AccountController(CommunicationProvider communicationProvider, ILocalizationService localizationService) : base(communicationProvider)
        {
            m_localizationService = localizationService;
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
        public IActionResult AccountSettings(AccountTab actualTab = AccountTab.UpdateAccount)
        {
            var viewmodel = CreateAccountDetailViewModel(actualTab);
            return View(viewmodel);
        }

        //
        // POST: /Account/UpdateAccount
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAccount(UpdateUserViewModel updateUserViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = GetRestClient())
                    {
                        var updateUserContract = new UpdateUserContract
                        {
                            FirstName = updateUserViewModel.FirstName,
                            LastName = updateUserViewModel.LastName
                        };

                        client.UpdateCurrentUser(updateUserContract);
                        ViewData.Add(AccountConstants.SuccessUserUpdate, true);
                    }
                }
                catch (HttpErrorCodeException e)
                {
                    ViewData.Add(AccountConstants.SuccessUserUpdate, false);
                    AddErrors(e);
                }
            }

            return PartialView("Settings/_UpdateAccount", updateUserViewModel);
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
                        return PartialView("Settings/_UpdatePassword", null);
                    }
                }
                catch (HttpErrorCodeException e)
                {
                    ViewData.Add(AccountConstants.SuccessPasswordUpdate, false);
                    AddErrors(e);
                }
            }

            return PartialView("Settings/_UpdatePassword", model);
        }

        //
        // POST: /Account/UpdateContact
        [HttpPost]
        public IActionResult UpdateContact([FromBody] UpdateUserContactContract updateUserContactContract)
        {
            try
            {
                if (string.IsNullOrEmpty(updateUserContactContract.NewContactValue))
                {
                    return AjaxErrorResponse(m_localizationService.Translate("EmptyEmail", "Account"), HttpStatusCode.BadRequest);
                }

                if (updateUserContactContract.NewContactValue == updateUserContactContract.OldContactValue)
                {
                    return AjaxErrorResponse(m_localizationService.Translate("SameEmail", "Account"), HttpStatusCode.BadRequest);
                }

                using (var client = GetRestClient())
                {
                    client.UpdateCurrentUserContact(updateUserContactContract);
                }
            }
            catch (HttpErrorCodeException e)
            {
                return AjaxErrorResponse(e.Message, e.StatusCode);
            }

            return AjaxOkResponse();
        }

        //
        // POST: /Account/ConfirmUserContact
        [HttpPost]
        public IActionResult ConfirmUserContact([FromBody] ConfirmUserContactRequest confirmUserContactRequest)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    var contract = new ConfirmUserContactContract
                    {
                        ConfirmCode = confirmUserContactRequest.ConfirmCode,
                        ContactType = confirmUserContactRequest.ContactType
                    };
                    var result = client.ConfirmUserContact(contract);
                    return Json(result);
                }
            }
            catch (HttpErrorCodeException e)
            {
                return AjaxErrorResponse(e.Message, e.StatusCode);
            }
        }

        //
        // POST: /Account/ResendConfirmCode
        [HttpPost]
        public IActionResult ResendConfirmCode([FromBody] ResendConfirmCodeRequest resendConfirmCodeRequest)
        {
            try
            {
                using (var client = GetRestClient())
                {
                    var contract = new UserContactContract
                    {
                        ContactType = resendConfirmCodeRequest.ContactType
                    };
                    client.ResendConfirmCode(contract);
                }
            }
            catch (HttpErrorCodeException e)
            {
                return AjaxErrorResponse(m_localizationService.Translate("ResendCodeError", "Account"), e.StatusCode);
            }

            return AjaxOkResponse();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTwoFactor(UpdateTwoFactorVerificationViewModel twoFactorVerificationViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = GetRestClient())
                {
                    try
                    {
                        var contract = new UpdateTwoFactorContract
                        {
                            TwoFactorIsEnabled = twoFactorVerificationViewModel.TwoFactorEnabled
                        };
                        client.SetTwoFactor(contract);
                        ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, true);
                    }
                    catch (HttpErrorCodeException e)
                    {
                        ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, false);
                        AddErrors(e);
                    }
                }
            }

            using (var client = GetRestClient())
            {
                var user = client.GetCurrentUser();
                twoFactorVerificationViewModel = Mapper.Map<UpdateTwoFactorVerificationViewModel>(user);
                return PartialView("Settings/_UpdateTwoFactorVerification", twoFactorVerificationViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeTwoFactorProvider(UpdateTwoFactorVerificationViewModel twoFactorVerificationViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = GetRestClient())
                {
                    try
                    {
                        var contract = new UpdateTwoFactorProviderContract
                        {
                            TwoFactorProvider = twoFactorVerificationViewModel.SelectedTwoFactorProvider
                        };
                        client.SelectTwoFactorProvider(contract);
                        ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, true);
                    }
                    catch (HttpErrorCodeException e)
                    {
                        ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, false);
                        AddErrors(e);
                    }
                }
            }

            using (var client = GetRestClient())
            {
                var user = client.GetCurrentUser();
                twoFactorVerificationViewModel = Mapper.Map<UpdateTwoFactorVerificationViewModel>(user);
                return PartialView("Settings/_UpdateTwoFactorVerification", twoFactorVerificationViewModel);
            }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private AccountDetailViewModel CreateAccountDetailViewModel(AccountTab accountTab = AccountTab.UpdateAccount)
        {
            using (var client = GetRestClient())
            {
                var user = client.GetCurrentUser();
                return new AccountDetailViewModel
                {
                    UpdateUserViewModel = Mapper.Map<UpdateUserViewModel>(user),
                    UpdatePasswordViewModel = null,
                    UpdateContactViewModel = Mapper.Map<UpdateContactViewModel>(user),
                    UpdateTwoFactorVerificationViewModel = Mapper.Map<UpdateTwoFactorVerificationViewModel>(user),
                    ActualTab = accountTab
                };
            }
        }
    }
}