using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using Microsoft.Net.Http.Server;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class AuthenticationManager
    {
        private const string OIDCUrl = "OIDCUrl";
        private const string OIDCClientId = "OIDCClientId";
        private const string OIDCClientSecret = "OIDCClientSecret";
        public string AuthToken { get; set; }

        public async Task SignInAsync()
        {
            // create a redirect URI using an available port on the loopback address.
            var redirectUri = "http://127.0.0.1:7890/";

            // create an HttpListener to listen for requests on that redirect URI.
            var settings = new WebListenerSettings();
            settings.UrlPrefixes.Add(redirectUri);
            var http = new WebListener(settings);

            http.Start();

            var options = new OidcClientOptions
            {
                Authority = ConfigurationManager.AppSettings[OIDCUrl],
                ClientId = ConfigurationManager.AppSettings[OIDCClientId],
                ClientSecret = ConfigurationManager.AppSettings[OIDCClientSecret],
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
            AuthToken = result.AccessToken;
        }

        private async Task SendResponseAsync(Response response)
        {
            var responseString = $"<html><head></head><body>Please return to the app.</body></html>";
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
            catch
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
