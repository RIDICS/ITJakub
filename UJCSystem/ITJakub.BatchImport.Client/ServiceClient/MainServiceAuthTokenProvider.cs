using Vokabular.MainService.DataContracts;

namespace ITJakub.BatchImport.Client.ServiceClient
{
    public class MainServiceAuthTokenProvider : IMainServiceAuthTokenProvider
    {
        public string AuthToken { get; set; }
    }
}