using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Core.Structures.Shared;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Attributes;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using AuthenticationManager = Vokabular.MainService.Core.Managers.AuthenticationManager;

namespace Vokabular.MainService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [CustomRequireHttps]
    [ValidateModel]
    public class UserController : BaseController
    {
        private readonly UserManager m_userManager;
        private readonly RoleManager m_roleManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly UserDetailManager m_userDetailManager;

        public UserController(UserManager userManager, RoleManager roleManager, AuthenticationManager authenticationManager,
            UserDetailManager userDetailManager)
        {
            m_userManager = userManager;
            m_roleManager = roleManager;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
        }

        [AllowAnonymous]
        [HttpPost("")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult CreateNewUser([FromBody] CreateUserContract data)
        {
            try
            {
                var userId = m_userManager.CreateNewUser(data);
                return Ok(userId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("external")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult CreateUserIfNotExist([FromBody] int externalId)
        {
            try
            {
                var userId = m_userManager.CreateUserIfNotExist(externalId);
                return Ok(userId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("current")]
        public UserDetailContract GetCurrentUser()
        {
            var user = m_authenticationManager.GetCurrentUser();
            return m_userDetailManager.GetUserDetailContractForUser(user);
        }

        [Authorize(PermissionNames.EditSelfPersonalData)]
        [HttpPut("current")]
        public IActionResult UpdateCurrentUser([FromBody] UpdateUserContract data)
        {
            try
            {
                m_userManager.UpdateCurrentUser(data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [Authorize(PermissionNames.EditSelfContacts)]
        [HttpPut("current/contact")]
        public IActionResult UpdateCurrentUserContacts([FromBody] UpdateUserContactContract data)
        {
            try
            {
                var user = m_authenticationManager.GetCurrentUser();
                m_userManager.UpdateUserContact(user.Id, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [HttpPut("current/password")]
        public IActionResult UpdateCurrentPassword([FromBody] UpdateUserPasswordContract data)
        {
            try
            {
                var userId = m_authenticationManager.GetCurrentUserId();
                m_userManager.UpdateUserPassword(userId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [Authorize(PermissionNames.ListUsers)]
        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<UserDetailContract> GetUserList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_userManager.GetUserList(start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [Authorize(PermissionNames.ListUsers)]
        [HttpGet("autocomplete")]
        public IList<UserDetailContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_userManager.GetUserAutocomplete(query, count);
            return result;
        }


        [HttpGet("{userId}/detail")]
        public UserDetailContract GetUserDetail(int userId)
        {
            var result = m_userManager.GetUserDetail(userId);
            return result;
        }

        [HttpGet("{userId}/role")]
        public List<RoleContract> GetRolesByUser(int userId)
        {
            var result = m_roleManager.GetRolesByUser(userId);
            return result;
        }

        [Authorize(PermissionNames.EditAnyUsersData)]
        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserContract data)
        {
            try
            {
                m_userManager.UpdateUser(userId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [Authorize(PermissionNames.EditAnyUsersData)]
        [HttpPut("{userId}/contact")]
        public IActionResult UpdateUserContact(int userId, [FromBody] UpdateUserContactContract data)
        {
            try
            {
                m_userManager.UpdateUserContact(userId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("current/contact/confirmation")]
        public IActionResult ConfirmContact([FromBody] ConfirmUserContactContract data)
        {
            try
            {
                var userId = m_authenticationManager.GetCurrentUserId();
                var result = m_userManager.ConfirmContact(userId, data);
                return Json(result);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("current/contact/confirmation/resend")]
        public IActionResult ResendConfirmCode([FromBody] UserContactContract data)
        {
            try
            {
                var userId = m_authenticationManager.GetCurrentUserId();
                m_userManager.ResendConfirmCode(userId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [Authorize(PermissionNames.SetTwoFactor)]
        [HttpPut("current/two-factor")]
        public IActionResult TwoFactor([FromBody] UpdateTwoFactorContract data)
        {
            try
            {
                var userId = m_authenticationManager.GetCurrentUserId();
                m_userManager.SetTwoFactor(userId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [Authorize(PermissionNames.SelectTwoFactorProvider)]
        [HttpPut("current/two-factor/provider")]
        public IActionResult TwoFactorProvider([FromBody] UpdateTwoFactorProviderContract data)
        {
            try
            { 
                var userId = m_authenticationManager.GetCurrentUserId();
                m_userManager.SelectTwoFactorProvider(userId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }
    }
}