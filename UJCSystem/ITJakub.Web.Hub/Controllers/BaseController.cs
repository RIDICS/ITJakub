using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Web.Hub.Identity;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly CommunicationProvider m_communication = new CommunicationProvider();


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

        private bool IsUserLoggedIn()
        {
            return User.Identity.IsAuthenticated;            
        }

        private string GetUserName()
        {
            return User.Identity.Name;
        }

        private string GetCommunicationToken()
        {
            var communicationToken = ClaimsPrincipal.Current.Claims.FirstOrDefault(x => x.Type == CustomClaimType.CommunicationToken);
            if (communicationToken == null)
                throw new ArgumentException("Cannot find communicationToken");

            return communicationToken.Value;
        }
    }
}