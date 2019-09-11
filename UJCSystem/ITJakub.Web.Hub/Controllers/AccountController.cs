using System.Net;
using System.Threading.Tasks;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Constants;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models.Requests.Permission;
using ITJakub.Web.Hub.Models.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.AspNetCore.Extensions;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly ILocalizationService m_localizationService;
        private readonly RefreshUserManager m_refreshUserManager;

        public AccountController(CommunicationProvider communicationProvider, ILocalizationService localizationService,
            RefreshUserManager refreshUserManager) : base(communicationProvider)
        {
            m_localizationService = localizationService;
            m_refreshUserManager = refreshUserManager;
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
                    var client = GetUserClient();
                    client.CreateNewUser(user);

                    return RedirectToAction(nameof(SuccessfulRegistration));
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
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
        // GET: /Account/UserProfile
        public IActionResult UserProfile(AccountTab actualTab = AccountTab.UpdateAccount)
        {
            var viewmodel = CreateAccountDetailViewModel(actualTab);
            return View(viewmodel);
        }

        //
        // POST: /Account/UpdateBasicData
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateBasicData(UpdateUserViewModel updateUserViewModel)
        {
            ViewData.Add(AccountConstants.SuccessUserUpdate, false);
            if (ModelState.IsValid)
            {
                try
                {
                    var client = GetUserClient();
                    var updateUserContract = new UpdateUserContract
                    {
                        FirstName = updateUserViewModel.FirstName,
                        LastName = updateUserViewModel.LastName
                    };

                    client.UpdateCurrentUser(updateUserContract);
                    ViewData.Add(AccountConstants.SuccessUserUpdate, true);
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
                {
                    AddErrors(e);
                }
            }

            return PartialView("UserProfile/_UpdateBasicData", updateUserViewModel);
        }

        //
        // POST: /Account/UpdatePassword
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(UpdatePasswordViewModel model)
        {
            ViewData.Add(AccountConstants.SuccessPasswordUpdate, false);
            if (ModelState.IsValid)
            {
                try
                {
                    var updateUserPasswordContract = new UpdateUserPasswordContract
                    {
                        NewPassword = model.Password,
                        OldPassword = model.OldPassword
                    };

                    var client = GetUserClient();
                    client.UpdateCurrentPassword(updateUserPasswordContract);
                    ViewData.Add(AccountConstants.SuccessPasswordUpdate, true);
                    return PartialView("UserProfile/_UpdatePassword", null);
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
                {
                    AddErrors(e);
                }
            }

            return PartialView("UserProfile/_UpdatePassword", model);
        }

        //
        // POST: /Account/UpdateContact
        [HttpPost]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateUserContactContract updateUserContactContract)
        {
            if (string.IsNullOrEmpty(updateUserContactContract.NewContactValue))
            {
                return AjaxErrorResponse(m_localizationService.Translate("EmptyEmail", "Account"), HttpStatusCode.BadRequest);
            }

            if (updateUserContactContract.NewContactValue == updateUserContactContract.OldContactValue)
            {
                return AjaxErrorResponse(m_localizationService.Translate("SameEmail", "Account"), HttpStatusCode.BadRequest);
            }

            var client = GetUserClient();
            client.UpdateCurrentUserContact(updateUserContactContract);
            await m_refreshUserManager.RefreshUserClaimsAsync(HttpContext);

            return AjaxOkResponse();
        }

        //
        // POST: /Account/ConfirmUserContact
        [HttpPost]
        public async Task<IActionResult> ConfirmUserContact([FromBody] ConfirmUserContactRequest confirmUserContactRequest)
        {
            var contract = new ConfirmUserContactContract
            {
                ConfirmCode = confirmUserContactRequest.ConfirmCode,
                ContactType = confirmUserContactRequest.ContactType
            };

            var client = GetUserClient();
            var result = client.ConfirmUserContact(contract);
            if (result)
            {
                await m_refreshUserManager.RefreshUserClaimsAsync(HttpContext);
            }

            return Json(result);
        }

        //
        // POST: /Account/ResendConfirmCode
        [HttpPost]
        public IActionResult ResendConfirmCode([FromBody] ResendConfirmCodeRequest resendConfirmCodeRequest)
        {
            try
            {
                var contract = new UserContactContract
                {
                    ContactType = resendConfirmCodeRequest.ContactType
                };

                var client = GetUserClient();
                client.ResendConfirmCode(contract);
                return AjaxOkResponse();
            }
            catch (HttpErrorCodeException e)
            {
                return AjaxErrorResponse(m_localizationService.Translate("ResendCodeError", "Account"), e.StatusCode);
            }
        }


        [HttpGet]
        public ActionResult TwoFactor()
        {
            var twoFactorVerificationViewModel = CreateUpdateTwoFactorVerificationViewModel();
            return PartialView("UserProfile/_UpdateTwoFactorVerification", twoFactorVerificationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTwoFactor(UpdateTwoFactorVerificationViewModel twoFactorVerificationViewModel)
        {
            ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, false);
            if (ModelState.IsValid)
            {
                try
                {
                    var contract = new UpdateTwoFactorContract
                    {
                        TwoFactorIsEnabled = twoFactorVerificationViewModel.TwoFactorEnabled
                    };
                    var client = GetUserClient();
                    client.SetTwoFactor(contract);
                    ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, true);
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
                {
                    AddErrors(e);
                }
            }

            twoFactorVerificationViewModel = CreateUpdateTwoFactorVerificationViewModel();
            return PartialView("UserProfile/_UpdateTwoFactorVerification", twoFactorVerificationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeTwoFactorProvider(UpdateTwoFactorVerificationViewModel twoFactorVerificationViewModel)
        {
            ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, false);
            if (ModelState.IsValid)
            {
                try
                {
                    var contract = new UpdateTwoFactorProviderContract
                    {
                        TwoFactorProvider = twoFactorVerificationViewModel.SelectedTwoFactorProvider
                    };

                    var client = GetUserClient();
                    client.SelectTwoFactorProvider(contract);
                    ViewData.Add(AccountConstants.SuccessTwoFactorUpdate, true);
                }
                catch (HttpErrorCodeException e)
                {
                    AddErrors(e);
                }
                catch (MainServiceException e)
                {
                    AddErrors(e);
                }
            }

            twoFactorVerificationViewModel = CreateUpdateTwoFactorVerificationViewModel();
            return PartialView("UserProfile/_UpdateTwoFactorVerification", twoFactorVerificationViewModel);
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
            var client = GetUserClient();
            var user = client.GetCurrentUser();
            return new AccountDetailViewModel
            {
                UpdateUserViewModel = Mapper.Map<UpdateUserViewModel>(user),
                UpdatePasswordViewModel = null,
                UpdateContactViewModel = Mapper.Map<UpdateContactViewModel>(user),
                UpdateTwoFactorVerificationViewModel = CreateUpdateTwoFactorVerificationViewModel(user),
                ActualTab = accountTab
            };
        }

        private UpdateTwoFactorVerificationViewModel CreateUpdateTwoFactorVerificationViewModel(UserDetailContract user = null)
        {
            if (user == null)
            {
                var client = GetUserClient();
                user = client.GetCurrentUser();
            }

            var updateTwoFactorVerificationViewModel = Mapper.Map<UpdateTwoFactorVerificationViewModel>(user);
            var isEmailConfirmed = User.IsEmailConfirmed();
            updateTwoFactorVerificationViewModel.IsEmailConfirmed = isEmailConfirmed ?? false;
            return updateTwoFactorVerificationViewModel;
        }
    }
}