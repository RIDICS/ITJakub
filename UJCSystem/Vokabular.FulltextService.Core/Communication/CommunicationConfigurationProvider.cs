using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Vokabular.Shared.Options;

namespace Vokabular.FulltextService.Core.Communication
{
    public class CommunicationConfigurationProvider : CommunicationConfigurationProviderBase
    {
        public CommunicationConfigurationProvider(IOptions<EndpointOption> endpointOptions) : base(endpointOptions)
        {
        }
    }
}