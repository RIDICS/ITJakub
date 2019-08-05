using System.IO;
using System.Net;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly CommunicationProvider m_communication;

        protected BaseController(CommunicationProvider communicationProvider)
        {
            m_communication = communicationProvider;
        }

        public MainServiceRestClient GetRestClient()
        {
            return m_communication.GetMainServiceClient();
        }

        public MainServiceProjectClient GetProjectClient()
        {
            return m_communication.GetMainServiceProjectClient();
        }

        public LemmatizationServiceClient GetLemmationzationServiceClient()
        {
            return m_communication.GetLemmatizationClient();
        }

        protected bool IsUserLoggedIn()
        {
            return User.Identity.IsAuthenticated;            
        }

        protected string GetUserName()
        {
            return User.Identity.Name;
        }
        
        protected JsonSerializerSettings GetJsonSerializerSettingsForBiblModule()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
        }

        protected FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName, long? fileSize)
        {
            Response.ContentLength = fileSize;
            return base.File(fileStream, contentType, fileDownloadName);
        }

        protected void AddErrors(HttpErrorCodeException exception)
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

        protected IActionResult AjaxOkResponse()
        {
            return new JsonResult(new {});
        }

        protected IActionResult AjaxErrorResponse(string message, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        {
            var result = new ErrorContract
            {
                ErrorMessage = message
            };

            return new ObjectResult(result)
            {
                StatusCode = (int)httpStatusCode
            };
        }
    }
}