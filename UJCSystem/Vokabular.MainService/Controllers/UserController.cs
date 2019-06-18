using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Attributes;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;

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

        [HttpPut("current/password")]
        public IActionResult UpdateCurrentPassword([FromBody] UpdateUserPasswordContract data)
        {
            try
            {
                m_userManager.UpdateUserPassword(data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<UserDetailContract> GetUserList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            var result = m_userManager.GetUserList(start, count, filterByName);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [Authorize(PermissionNames.ManagePermissions)]
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

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("autocomplete")]
        public IList<UserDetailContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_userManager.GetUserAutocomplete(query, count);
            return result;
        }
    }
}