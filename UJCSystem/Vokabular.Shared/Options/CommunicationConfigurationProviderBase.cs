using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Extensions.Options;

namespace Vokabular.Shared.Options
{
    public class CommunicationConfigurationProviderBase
    {
        private readonly Dictionary<string, EndpointOption> m_endpointsOptions;

        public CommunicationConfigurationProviderBase(IOptions<List<EndpointOption>> endpointOptions)
        {
            m_endpointsOptions = endpointOptions.Value.ToDictionary(x => x.Name);
        }

        public Uri GetEndpointUri(string name)
        {
            var options = m_endpointsOptions[name];
            return new Uri(options.Address);
        }

        public EndpointAddress GetEndpointAddress(string name)
        {
            var options = m_endpointsOptions[name];
            return new EndpointAddress(options.Address);
        }
    }
}