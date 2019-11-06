using Vokabular.MainService.DataContracts;

namespace Vokabular.AppAuthentication.Shared.ServiceClient
{
    public class MainServiceAuthTokenProvider : IMainServiceAuthTokenProvider
    {
        public string AuthToken { get; set; }
    }
}