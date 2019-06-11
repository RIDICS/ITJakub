using System.Collections.Generic;

namespace Vokabular.Authentication.Client.SharedClient.Config
{
    public class OpenIdConnectConfig
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public IList<string> Scopes { get; set; }
    }
}