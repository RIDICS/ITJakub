using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Vokabular.Authentication.TicketStore
{
    public class PostConfigureCookieAuthenticationOptions : IPostConfigureOptions<CookieAuthenticationOptions>
    {
        private readonly ITicketStore m_ticketStore;

        public PostConfigureCookieAuthenticationOptions(ITicketStore ticketStore)
        {
            m_ticketStore = ticketStore;
        }

        public void PostConfigure(string name, CookieAuthenticationOptions options)
        {
            options.SessionStore = m_ticketStore;
        }
    }
}