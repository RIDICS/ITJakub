using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Facebook;
using ITJakub.MobileApps.MainApp.JSON;
using ITJakub.MobileApps.MainApp.ViewModel;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.MainApp.Manager
{
    public class LoginManager
    {
        public async Task<UserInfo> LoginLiveId()
        {
            return null;
            /*
            const bool signIn = false;
            try
            {
                // Open Live Connect SDK client.
                var LCAuth = new LiveAuthClient();
                var LCLoginResult = await LCAuth.InitializeAsync();
                try
                {
                    LiveLoginResult loginResult = null;
                    if (signIn)
                    {
                        // Sign in to the user's Microsoft account with the required scope.
                        //  
                        //  This call will display the Microsoft account sign-in screen if 
                        //   the user is not already signed in to their Microsoft account 
                        //   through Windows 8.
                        // 
                        //  This call will also display the consent dialog, if the user has 
                        //   has not already given consent to this app to access the data 
                        //   described by the scope.
                        // 
                        //  Change the parameter of LoginAsync to include the scopes 
                        //   required by your app.
                        loginResult = await LCAuth.LoginAsync(new string[] { "wl.basic" });
                    }
                    else
                    {
                        // If we don't want the user to sign in, continue with the current 
                        //  sign-in state.
                        loginResult = LCLoginResult;
                    }
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        // Create a client session to get the profile data.
                        var connect = new LiveConnectClient(LCAuth.Session);

                        // Get the profile info of the user.
                        var operationResult = await connect.GetAsync("me");
                        dynamic result = operationResult.Result;
                        if (result != null)
                        {
                            // Update the text of the object passed in to the method. 
                            Message = string.Join(" ", "Hello", result.name, "!");
                        }
                        else
                        {
                            // Handle the case where the user name was not returned. 
                        }
                    }
                    else
                    {
                        // The user hasn't signed in so display this text 
                        //  in place of his or her name.
                        Message = "You're not signed in.";
                    }
                }
                catch (LiveAuthException exception)
                {
                    // Handle the exception. 
                }
            }
            catch (LiveAuthException exception)
            {
                // Handle the exception. 
            }
            catch (LiveConnectException exception)
            {
                // Handle the exception. 
            }
             */
        }

        public async Task<UserInfo> LoginFacebookAsync()
        {
            var fbClient = new FacebookClient();

            var loginUrl = fbClient.GetLoginUrl(new
            {
                client_id = ***REMOVED***,
                redirect_uri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri,
                scope = "email",
                display = "popup",
                response_type = "token"
            });

            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                  WebAuthenticationOptions.None,
                  loginUrl);

            var userInfo = new UserInfo();

            switch (webAuthenticationResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    var callbackUri = new Uri(webAuthenticationResult.ResponseData.ToString());
                    var facebookOAuthResult = fbClient.ParseOAuthCallbackUrl(callbackUri);

                    // Retrieve the Access Token. You can now interact with Facebook on behalf of the user
                    // using the Access Token.
                    var accessToken = facebookOAuthResult.AccessToken;

                    fbClient = new FacebookClient(accessToken);
                    var user = await fbClient.GetTaskAsync("/me");
                    var result = (IDictionary<string, object>)user;

                    userInfo.Success = true;
                    userInfo.AccessToken = accessToken;
                    userInfo.Email = result["email"].ToString();
                    userInfo.FirstName = result["first_name"].ToString();
                    userInfo.LastName = result["last_name"].ToString();
                    break;
                case WebAuthenticationStatus.ErrorHttp:
                    // handle authentication failure
                    userInfo.Success = false;
                    break;
                default:
                    // The user canceled the authentication
                    userInfo.Success = false;
                    break;
            }
            return userInfo;
        }

        public async Task<UserInfo> LoginGoogle()
        {
            var startUri = new Uri("https://accounts.google.com/o/oauth2/auth?client_id=***REMOVED***&response_type=code&scope=openid%20email%20profile&redirect_uri=urn:ietf:wg:oauth:2.0:oob");
            var endUri = new Uri("https://accounts.google.com/o/oauth2/approval?");
            var userInfo = new UserInfo();
            string resultSring;

            try
            {
                var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
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
                        resultSring = webAuthenticationResult.ResponseErrorDetail.ToString();
                        userInfo.Success = false;
                        break;
                    default:
                        // Other error.
                        resultSring = webAuthenticationResult.ResponseData;
                        userInfo.Success = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                // Authentication failed. Handle parameter, SSL/TLS, and Network Unavailable errors here. 
                resultSring = ex.Message;
                userInfo.Success = false;
            }

            if (!userInfo.Success)
            {
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
            string code = null;
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
                new KeyValuePair<string, string>("client_id", "***REMOVED***"),
                new KeyValuePair<string, string>("client_secret", "***REMOVED***"),
                new KeyValuePair<string, string>("redirect_uri", "urn:ietf:wg:oauth:2.0:oob"),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            };
            var info = await client.PostAsync(new Uri("https://accounts.google.com/o/oauth2/token"), new HttpFormUrlEncodedContent(parameters));

            return JsonConvert.DeserializeObject<GoogleToken>(info.Content.ToString());
        }

        private async Task GetGoogleUserInfoAsync(UserInfo userInfo, GoogleToken googleTokens)
        {
            var client = new HttpClient();

            // Get e-mail
            var tokenInfo = await client.GetAsync(new Uri(String.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?id_token={0}", googleTokens.IdToken)));
            var idToken = JsonConvert.DeserializeObject<GoogleIdToken>(tokenInfo.Content.ToString());

            userInfo.Email = idToken.Email;

            // Get name
            client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", googleTokens.AccessToken);
            var userInfoResponse = await client.GetAsync(new Uri("https://www.googleapis.com/plus/v1/people/me"));
            var googleUserInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoResponse.Content.ToString());

            userInfo.FirstName = googleUserInfo.Name.GivenName;
            userInfo.LastName = googleUserInfo.Name.FamilyName;
        }
    }
}
