using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker;
using ITJakub.MobileApps.Client.DataContracts.Json;
using ITJakub.MobileApps.DataContracts;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public class GoogleProvider : ILoginProvider
    {
        private const string ClientId = "***REMOVED***";
        private const string ClientSecret = "***REMOVED***";
        private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        private const string StartUri = "https://accounts.google.com/o/oauth2/auth?client_id={0}&response_type=code&scope=openid%20email%20profile&redirect_uri={1}";
        private const string EndUri = "https://accounts.google.com/o/oauth2/approval?";
        private const string TokenUrl = "https://accounts.google.com/o/oauth2/token";
        private const string TokenInfoUrl = "https://www.googleapis.com/oauth2/v1/tokeninfo?id_token={0}";
        private const string UserInfoUrl = "https://www.googleapis.com/plus/v1/people/me";

        public string AccountName { get { return "Google"; } }
        public AuthProvidersContract ProviderType { get { return AuthProvidersContract.Google; } }

        public async Task<UserLoginSkeleton> LoginAsync()
        {
            var startUri = new Uri(string.Format(StartUri, ClientId, RedirectUri));
            var endUri = new Uri(EndUri);
            var userInfo = new UserLoginSkeleton();
            string resultSring;

            try
            {
                var webAuthenticationResult = await CustomWebAuthenticationBroker.AuthenticateAsync(
                    WebAuthenticationOptions.UseTitle,
                    startUri,
                    endUri);

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


            var googleTokens = await GetGoogleTokensAsync(resultSring);
            userInfo.AccessToken = googleTokens.AccessToken;

            await GetGoogleUserInfoAsync(userInfo, googleTokens);
            return userInfo;
        }

        private async Task<GoogleToken> GetGoogleTokensAsync(string authenticationResponse)
        {
            const string successString = "Success ";
            string code;
            try
            {
                if (authenticationResponse.StartsWith(successString))
                {
                    authenticationResponse = authenticationResponse.TrimStart(successString.ToCharArray());
                }

                var decoder = new WwwFormUrlDecoder(authenticationResponse);
                code = decoder.GetFirstValueByName("code");
            }
            catch (ArgumentException)
            {
                //parameter "code" doesn't exists
                return null;
            }

            var client = new HttpClient();
            var parameters = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            };
            var info = await client.PostAsync(new Uri(TokenUrl), new HttpFormUrlEncodedContent(parameters));

            return JsonConvert.DeserializeObject<GoogleToken>(info.Content.ToString());
        }

        private async Task GetGoogleUserInfoAsync(UserLoginSkeleton userLoginSkeleton, GoogleToken googleTokens)
        {
            var client = new HttpClient();

            // Get e-mail
            var tokenInfo = await client.GetAsync(new Uri(String.Format(TokenInfoUrl, googleTokens.IdToken)));
            var idToken = JsonConvert.DeserializeObject<GoogleIdToken>(tokenInfo.Content.ToString());

            userLoginSkeleton.Email = idToken.Email;

            // Get name
            client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", googleTokens.AccessToken);
            var userInfoResponse = await client.GetAsync(new Uri(UserInfoUrl));
            var googleUserInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoResponse.Content.ToString());

            userLoginSkeleton.FirstName = googleUserInfo.Name.GivenName;
            userLoginSkeleton.LastName = googleUserInfo.Name.FamilyName;
        }

    }
}
