namespace ITJakub.Web.Hub.Models.Config
{
    public class OpenIdConnectConfiguration
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthServiceScopeName { get; set; }
    }
}
