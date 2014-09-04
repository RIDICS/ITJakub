using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public class LiveIdProvider : ILoginProvider
    {
        private const string ClientId = "***REMOVED***";
        private const string ClientSecret = "***REMOVED***";
        private const string StartUri = "https://login.live.com/oauth20_authorize.srf?client_id={0}&scope=wl.basic&response_type=token&redirect_uri={1}";

        public string AccountName { get { return "Live ID"; } }
        public AuthProvidersContract ProviderType { get { return AuthProvidersContract.LiveId; } }

        //TODO register this application in Windows Store developer account and test this method
        /*
        public async Task<UserLoginSkeleton> LoginLiveId()
        {
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

                        // Get the profile loginSkeleton of the user.
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
        }
        */

        public async Task<UserLoginSkeleton> LoginAsync()
        {
            var endUriString = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri;
            var startUriString = string.Format(StartUri, ClientId, endUriString);
            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(startUriString),
                WebAuthenticationBroker.GetCurrentApplicationCallbackUri());


            return new UserLoginSkeleton
            {
                Success = true,
                FirstName = "Mocked",
                LastName = "User",
                AccessToken = "Aaaaaaaaa",
                Email = "email@example.com"
            };
        }
    }
}