using System.IO;
using System.Net;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Helpers;
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
        private readonly HttpErrorCodeTranslator m_httpErrorCodeTranslator;

        protected BaseController(CommunicationProvider communicationProvider, HttpErrorCodeTranslator httpErrorCodeTranslator)
        {
            m_communication = communicationProvider;
            m_httpErrorCodeTranslator = httpErrorCodeTranslator;
        }

        public MainServiceRestClient GetRestClient()
        {
            return m_communication.GetMainServiceClient();
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
                var message = exception.Message;
                if (string.IsNullOrEmpty(message))
                {
                    message = m_httpErrorCodeTranslator.GetMessageFromErrorCode(exception.StatusCode).ErrorMessage;
                }
                ModelState.AddModelError(string.Empty, message);
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
            ErrorContract errorContract;

            if (string.IsNullOrEmpty(message))
            {
                errorContract = m_httpErrorCodeTranslator.GetMessageFromErrorCode(httpStatusCode);
            }
            else
            {
                errorContract = new ErrorContract
                {
                    ErrorMessage = message
                };
            }

            return new ObjectResult(errorContract)
            {
                StatusCode = (int)httpStatusCode
            };
        }
    }
}