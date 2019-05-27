namespace Vokabular.MainService
{
    public class OpenIdConnect
    {
        public string Url { get; set; }
        public string UserInfoEndpoint { get; set; }
        public string AuthServiceScopeName { get; set; }
        public string ApiKey { get; set; }
        public string ApiKeyHeader { get; set; }
    }
}