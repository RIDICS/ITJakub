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
        private const string EncryptedEndpointName = "ItJakubServiceEncrypted";
        private const string MainServiceEndpointName= "ItJakubService";
        private const string MainServiceEndpointNameAuthenticated = "ItJakubService.Authenticated";
        private const string StreamedServiceEndpointName = "ItJakubServiceStreamed";
        private const string StreamedServiceEndpointNameAuthenticated = "ItJakubServiceStreamed.Authenticated";


        protected IItJakubService GetAuthenticatedClient()
        {
            var client = new ItJakubServiceClient("AuthenticatedEndpoint");
            if (client.ClientCredentials == null)
            {
                throw new ArgumentException("Cannot set credentials for client");
            }
            client.ClientCredentials.UserName.UserName = GetUserName();
            client.ClientCredentials.UserName.Password = GetCommunicationToken();

            return client;
        }

        protected ItJakubServiceEncryptedClient GetEncryptedClient()
        {
            var client = new ItJakubServiceEncryptedClient(EncryptedEndpointName);
            return client;
        }

        protected ItJakubServiceStreamedClient GetStreamingClient()
        {
            var client = new ItJakubServiceStreamedClient();
            return client;
        }

        protected IItJakubService GetUnsecuredClient()
        {
            var client = new ItJakubServiceClient();
            return client;
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