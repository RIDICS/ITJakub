using System;
using System.Linq;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Identity;
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

        public ItJakubServiceEncryptedClient GetEncryptedClient()
        {
            return m_communication.GetEncryptedClient();
        }

        public IItJakubService GetMainServiceClient()
        {
            if (!IsUserLoggedIn()) return m_communication.GetUnsecuredClient();

            var username = GetUserName();
            var password = GetCommunicationToken();

            return m_communication.GetAuthenticatedClient(username, password);
        }

        public ItJakubServiceStreamedClient GetStreamingClient()
        {
            if (!IsUserLoggedIn()) return m_communication.GetStreamingClient();

            var username = GetUserName();
            var password = GetCommunicationToken();

            return m_communication.GetStreamingClientAuthenticated(username, password);
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

        private string GetCommunicationToken()
        {
            var communicationToken = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.CommunicationToken);
            if (communicationToken == null)
                throw new ArgumentException("Cannot find communicationToken");

            return communicationToken.Value;
        }
        
        protected JsonSerializerSettings GetJsonSerializerSettingsForBiblModule()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
        }
    }
}