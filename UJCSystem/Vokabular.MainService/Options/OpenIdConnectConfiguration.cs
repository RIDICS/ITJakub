namespace Vokabular.MainService.Options
{
    public class OpenIdConnectConfiguration
    {
        public string Url { get; set; }
        public string AuthServiceScopeName { get; set; }
        
        public string ClientSecret { get; set; }
        public string ClientId { get; set; }
    }
}