using System.IO;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vokabular.MainService.DataContracts.Clients;

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
    }
}