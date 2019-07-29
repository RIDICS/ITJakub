using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Extensions.Options;

namespace Vokabular.Shared.Options
{
    public abstract class CommunicationConfigurationProviderBase
    {
        private readonly IDictionary<string, string> m_endpointAddresses;

        public CommunicationConfigurationProviderBase(IOptions<EndpointOption> endpointOptions)
        {
            m_endpointAddresses = endpointOptions.Value.Addresses;
        }

        public Uri GetEndpointUri(string name)
        {
            var url = m_endpointAddresses[name];
            return new Uri(url);
        }

        public EndpointAddress GetEndpointAddress(string name)
        {
            var url = m_endpointAddresses[name];
            return new EndpointAddress(url);
        }
    }
}