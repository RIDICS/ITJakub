using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Attributes;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    [CustomRequireHttps]
    public class AuthTokenController : BaseController
    {
        private readonly AuthenticationManager m_authenticationManager;

        public AuthTokenController(AuthenticationManager authenticationManager)
        {
            m_authenticationManager = authenticationManager;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(SignInResultContract), StatusCodes.Status200OK)]
        public IActionResult SignIn([FromBody] SignInContract data)
        {
            try
            {
                var result = m_authenticationManager.SignIn(data);
                return Ok(result);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("")]
        public IActionResult SignOut([FromHeader(Name = CustomHttpHeaders.Authorization)] string authorizationToken)
        {
            try
            {
                m_authenticationManager.SignOut();
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode(exception.StatusCode, exception.Message);
            }
        }
    }
}