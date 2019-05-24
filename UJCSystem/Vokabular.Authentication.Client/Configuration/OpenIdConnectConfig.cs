namespace Vokabular.Authentication.Client.Configuration
{
    public class OpenIdConnectConfig
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthServiceScopeName { get; set; }
    }
}