using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using ITJakub.MobileApps.Client.DataContracts.Json;
using ITJakub.MobileApps.DataContracts;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public class LiveIdProvider : ILoginProvider
    {
        private const string ClientId = "***REMOVED***";
        private const string StartUri = "https://login.live.com/oauth20_authorize.srf?client_id={0}&scope=wl.basic%20wl.emails&response_type=token";
        private const string RedirectUri = "http://b2191704-17ad-43e2-9e49-ecf66c89ed23.apps.dev.live.com/";
        private const string UserInfoUrl = "https://apis.live.net/v5.0/me?access_token={0}";

        public string AccountName { get { return "Microsoft"; } }
        public AuthProvidersContract ProviderType { get { return AuthProvidersContract.LiveId; } }

        public async Task<UserLoginSkeleton> LoginAsync()
        {
            var userInfo = new UserLoginSkeleton();
            var startUriString = string.Format(StartUri, ClientId);
            string resultSring;

            try
            {
                var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(startUriString),
                    new Uri(RedirectUri));

                switch (webAuthenticationResult.ResponseStatus)
                {
                    case WebAuthenticationStatus.Success:
                        // Successful authentication. 
                        resultSring = webAuthenticationResult.ResponseData;
                        userInfo.Success = true;
                        break;
                    case WebAuthenticationStatus.ErrorHttp:
                        // HTTP error. 
                        // resultSring = webAuthenticationResult.ResponseErrorDetail.ToString();
                        userInfo.Success = false;
                        return userInfo;
                    default:
                        // Other error.
                        // resultSring = webAuthenticationResult.ResponseData;
                        userInfo.Success = false;
                        return userInfo;
                }
            }
            catch (Exception)
            {
                // Authentication failed. Handle parameter, SSL/TLS, and Network Unavailable errors here. 
                // resultSring = ex.Message;
                userInfo.Success = false;
                return userInfo;
            }

            var accessToken = GetAccessToken(resultSring);
            if (accessToken == null)
                return userInfo;

            userInfo.AccessToken = accessToken;
            await GetUserInfo(userInfo, accessToken);

            return userInfo;
        }

        private string GetAccessToken(string url)
        {
            if (url == null)
                return null;

            var parameterIndex = url.IndexOf('#');
            if (parameterIndex == -1)
                return null;

            var parameter = url.Substring(parameterIndex + 1);

            try
            {
                var decoder = new WwwFormUrlDecoder(parameter);
                return decoder.GetFirstValueByName("access_token");
            }
            catch (ArgumentException)
            {
                //parameter "access_token" doesn't exists
                return null;
            }
        }

        private async Task GetUserInfo(UserLoginSkeleton userInfo, string accessToken)
        {
            var client = new HttpClient();

            var userInfoResponse = await client.GetAsync(new Uri(String.Format(UserInfoUrl, accessToken)));
            var liveIdUserInfo = JsonConvert.DeserializeObject<LiveIdUserInfo>(userInfoResponse.Content.ToString());

            userInfo.FirstName = liveIdUserInfo.FirstName;
            userInfo.LastName = liveIdUserInfo.LastName;
            userInfo.Email = liveIdUserInfo.Emails.Account;
        }
    }
}