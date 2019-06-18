using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Vokabular.Authentication.Client.SharedClient.Authentication.Events;

namespace Vokabular.Authentication.Client.SharedClient.Authentication.Options
{
    public class AutomaticTokenManagementConfigureCookieOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        private readonly AuthenticationScheme m_scheme;

        public AutomaticTokenManagementConfigureCookieOptions(IAuthenticationSchemeProvider provider)
        {
            m_scheme = provider.GetDefaultSignInSchemeAsync().GetAwaiter().GetResult();
        }

        public void Configure(CookieAuthenticationOptions options)
        { }

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            if (name == m_scheme.Name)
            {
                options.EventsType = typeof(AutomaticTokenManagementCookieEvents);
            }
        }
    }
}