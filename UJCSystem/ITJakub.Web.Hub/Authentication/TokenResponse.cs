using Newtonsoft.Json.Linq;

namespace ITJakub.Web.Hub.Authentication
{
    public class TokenResponse
    {
        private readonly JObject m_json;

        public TokenResponse(JObject json)
        {
            this.m_json = json;
        }
       
        public string AccessToken => TryGet(OidcConstants.AccessToken);

        public string IdentityToken => TryGet(OidcConstants.IdentityToken);

        public string TokenType => TryGet(OidcConstants.TokenType);

        public string RefreshToken => TryGet(OidcConstants.RefreshToken);

        public string ErrorDescription => TryGet(OidcConstants.ErrorDescription);

        public int ExpiresIn
        {
            get
            {
                var value = TryGet(OidcConstants.ExpiresIn);

                if (value != null)
                {
                    if (int.TryParse(value, out var theValue))
                    {
                        return theValue;
                    }
                }

                return 0;
            }
        }

        private string TryGet(string name) => TryGetString(name);

        private string TryGetString(string name)
        {
            m_json.TryGetValue(name, out var value);
            return value?.ToString();
        }
    }
}
