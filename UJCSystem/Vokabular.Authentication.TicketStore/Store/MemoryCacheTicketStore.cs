using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Vokabular.Authentication.TicketStore.Store
{
    // Source implementation: https://github.com/aspnet/Security/blob/master/samples/CookieSessionSample/MemoryCacheTicketStore.cs
    public class MemoryCacheTicketStore : ITicketStore
    {
        private readonly IMemoryCache m_memoryCache;
        private readonly CacheTicketStoreConfig m_config;

        public MemoryCacheTicketStore(IMemoryCache memoryCache, CacheTicketStoreConfig config)
        {
            m_memoryCache = memoryCache;
            m_config = config;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var cacheKey = GenerateEntryKey();

            await RenewAsync(cacheKey, ticket);

            return cacheKey;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new MemoryCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }
            options.SetSlidingExpiration(m_config.SlidingExpiration);
            m_memoryCache.Set(key, ticket, options);

            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var ticketWasPresent = m_memoryCache.TryGetValue(key, out AuthenticationTicket ticket);

            return ticketWasPresent ? Task.FromResult(ticket) : Task.FromResult<AuthenticationTicket>(null);
        }

        public Task RemoveAsync(string key)
        {
            m_memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        private string GenerateEntryKey()
        {
            var entryKey = Guid.NewGuid();
            return entryKey.ToString();
        }
    }
}