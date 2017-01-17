using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Extensions.Options;
using Vokabular.Shared.Options;

namespace Vokabular.MainService.Core.Communication
{
    public class CommunicationConfigurationProvider : CommunicationConfigurationProviderBase
    {
        public CommunicationConfigurationProvider(IOptions<List<EndpointOption>> endpointOptions) : base(endpointOptions)
        {
        }

        public BasicHttpBinding GetBasicHttpBindingStreamed()
        {
            return new BasicHttpBinding
            {
                MaxBufferPoolSize = 2097120,
                MaxBufferSize = 65536,
                MaxReceivedMessageSize = 2147483647,
                CloseTimeout = new TimeSpan(0, 25, 0),
                OpenTimeout = new TimeSpan(0, 25, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                TransferMode = TransferMode.Streamed
            };
        }
    }
}