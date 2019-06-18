using System;

namespace Vokabular.Authentication.TicketStore.Store
{
    public class CacheTicketStoreConfig
    {
        public TimeSpan SlidingExpiration { get; set; }
    }
}