using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Utils.Documentation;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly UserManager m_userManager;

        public UserController(UserManager userManager)
        {
            m_userManager = userManager;
        }

        [HttpPost("")]
        public int CreateNewUser([FromBody] CreateUserContract data)
        {
            var userId = m_userManager.CreateNewUser(data);
            return userId;
        }

        [HttpGet("current")]
        public UserDetailContract GetCurrentUser(
            [FromHeader(Name = CustomHttpHeaders.Authorization)] string authorizationToken)
        {
            throw new NotImplementedException();
        }

        [HttpPut("current")]
        public void UpdateUser([FromBody] UpdateUserContract data,
            [FromHeader(Name = CustomHttpHeaders.Authorization)] string authorizationToken)
        {
            throw new NotImplementedException();
        }

        [HttpPut("current/password")]
        public void UpdatePassword([FromBody] UpdateUserPasswordContract data,
            [FromHeader(Name = CustomHttpHeaders.Authorization)] string authorizationToken)
        {
            throw new NotImplementedException();
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<UserDetailContract> GetUserList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByName)
        {
            SetTotalCountHeader(0);
            throw new NotImplementedException();
        }
    }

    [Route("api/[controller]")]
    public class AuthTokenController : BaseController
    {
        [HttpPost("")]
        public void SignIn([FromBody] SignInContract data)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("")]
        public void SignOut([FromHeader(Name = CustomHttpHeaders.Authorization)] string authorizationToken)
        {
            throw new NotImplementedException();
        }
    }
}
