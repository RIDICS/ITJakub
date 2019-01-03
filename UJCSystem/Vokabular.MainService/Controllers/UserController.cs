using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.Extensions;
using Vokabular.Shared.AspNetCore.WebApiUtils.Attributes;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    [CustomRequireHttps]
    [ValidateModel]
    public class UserController : BaseController
    {
        private readonly UserManager m_userManager;
        private readonly PermissionManager m_permissionManager;
        private readonly UserGroupManager m_userGroupManager;

        public UserController(UserManager userManager, PermissionManager permissionManager, UserGroupManager userGroupManager)
        {
            m_userManager = userManager;
            m_permissionManager = permissionManager;
            m_userGroupManager = userGroupManager;
        }

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

        [HttpPut("current")]
        public IActionResult UpdateCurrentUser([FromBody] UpdateUserContract data,
            [FromHeader(Name = CustomHttpHeaders.Authorization)] string authorizationToken)
        {
            try
            {
                m_userManager.UpdateUser(data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [HttpPut("current/password")]
        public IActionResult UpdateCurrentPassword([FromBody] UpdateUserPasswordContract data,
            [FromHeader(Name = CustomHttpHeaders.Authorization)] string authorizationToken)
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
        
        [HttpGet("{userId}/group")]
        public List<UserGroupContract> GetGroupsByUser(int userId)
        {
            var result = m_userGroupManager.GetGroupsByUser(userId);
            return result;
        }

        [HttpGet("current/permission/special")]
        public IList<SpecialPermissionContract> GetSpecialPermissionsForUser(SpecialPermissionCategorizationEnumContract? filterByType)
        {
            var result = m_permissionManager.GetSpecialPermissionsForUser(filterByType);
            return result;
        }

        [Authorize(PermissionNames.ManagePermissions)]
        [HttpGet("autocomplete")]
        public List<UserDetailContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_userManager.GetUserAutocomplete(query, count);
            return result;
        }
    }
}
