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

        public MainServiceBookClient GetBookClient()
        {
            return m_communication.GetMainServiceBookClient();
        }

        public MainServiceCategoryClient GetCategoryClient()
        {
            return m_communication.GetMainServiceCategoryClient();
        }

        public MainServiceExternalRepositoryClient GetExternalRepositoryClient()
        {
            return m_communication.GetMainServiceExternalRepositoryClient();
        }
        
        public MainServiceFavoriteClient GetFavoriteClient()
        {
            return m_communication.GetMainServiceFavoriteClient();
        }

        public MainServiceFilteringExpressionSetClient GetFilteringExpressionSetClient()
        {
            return m_communication.GetMainServiceFilteringExpressionSetClient();
        }

        public MainServiceMetadataClient GetMetadataClient()
        {
            return m_communication.GetMainServiceMetadataClient();
        }

        public MainServiceProjectClient GetProjectClient()
        {
            return m_communication.GetMainServiceProjectClient();
        }

        public MainServiceResourceClient GetResourceClient()
        {
            return m_communication.GetMainServiceResourceClient();
        }

        public MainServiceRoleClient GetRoleClient()
        {
            return m_communication.GetMainServiceRoleClient();
        }

        public MainServiceUserClient GetUserClient()
        {
            return m_communication.GetMainServiceUserClient();
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