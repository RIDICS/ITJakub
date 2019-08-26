using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using ITJakub.BatchImport.Client.ServiceClient;
using Microsoft.Net.Http.Server;
using Vokabular.MainService.DataContracts;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class AuthenticationManager
    {
        private readonly MainServiceAuthTokenProvider m_authTokenProvider;
        private const string OidcUrl = "OIDCUrl";
        private const string OidcClientId = "OIDCClientId";
        private const string OidcClientSecret = "OIDCClientSecret";

        public AuthenticationManager(IMainServiceAuthTokenProvider authTokenProvider)
        {
            m_authTokenProvider = (MainServiceAuthTokenProvider) authTokenProvider;
        }

        public async Task SignInAsync()
        {
            // create a redirect URI using an available port on the loopback address.
            var redirectUri = "http://127.0.0.1:7890/";

            // create an HttpListener to listen for requests on that redirect URI.
            var settings = new WebListenerSettings();
            settings.UrlPrefixes.Add(redirectUri);
            using (var http = new WebListener(settings))
            {
                http.Start();

                var options = new OidcClientOptions
                {
                    Authority = ConfigurationManager.AppSettings[OidcUrl],
                    ClientId = ConfigurationManager.AppSettings[OidcClientId],
                    ClientSecret = ConfigurationManager.AppSettings[OidcClientSecret],
                    RedirectUri = redirectUri,
                    Scope = "openid profile",
                    FilterClaims = true,
                    LoadProfile = true,
                    Flow = OidcClientOptions.AuthenticationFlow.Hybrid
                };

                var client = new OidcClient(options);
                var state = await client.PrepareLoginAsync();

                OpenBrowser(state.StartUrl);

                var context = await http.AcceptAsync();
                var formData = GetRequestPostData(context.Request);

                if (formData == null)
                {
                    throw new AuthenticationException("Invalid response");
                }

                await SendResponseAsync(context.Response);

                var result = await client.ProcessResponseAsync(formData, state);

                if (result.IsError)
                {
                    throw new AuthenticationException(result.Error);
                }
                m_authTokenProvider.AuthToken = result.AccessToken;
            }
        }

        private async Task SendResponseAsync(Response response)
        {
            var responseString = "<html><head></head><body>Please return to the Batch Import client app.</body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentLength = buffer.Length;

            var responseOutput = response.Body;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Flush();
        }

        private void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception e) when (e is InvalidOperationException || e is Win32Exception || e is PlatformNotSupportedException)
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private string GetRequestPostData(Request request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var body = request.Body)
            {
                using (var reader = new StreamReader(body))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
