using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ITJakub.Web.Hub.Models.Options;
using Microsoft.Extensions.Options;

namespace ITJakub.Web.Hub.Managers
{
    public class CommunicationConfigurationProvider
    {
        private readonly Dictionary<string, EndpointOption> m_endpointsOptions;

        public CommunicationConfigurationProvider(IOptions<List<EndpointOption>> endpointOptions)
        {
            m_endpointsOptions = endpointOptions.Value.ToDictionary(x => x.Name);
        }

        public EndpointAddress GetEndpointAddress(string name)
        {
            var options = m_endpointsOptions[name];
            return new EndpointAddress(options.Address);
        }

        public BasicHttpBinding GetBasicHttpBinding()
        {
            return new BasicHttpBinding
            {
                MaxBufferPoolSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                CloseTimeout = new TimeSpan(0, 10, 0),
                OpenTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                {
                    MaxDepth = 2000000,
                    MaxStringContentLength = 2147483647,
                    MaxArrayLength = 2147483647,
                    MaxBytesPerRead = 2147483647,
                    MaxNameTableCharCount = 2147483647
                }
            };
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

        public BasicHttpsBinding GetBasicHttpsBindingUserNameAuthentication()
        {
            return new BasicHttpsBinding
            {
                MaxBufferPoolSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                CloseTimeout = new TimeSpan(0, 10, 0),
                OpenTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                {
                    MaxDepth = 2000000,
                    MaxStringContentLength = 2147483647,
                    MaxArrayLength = 2147483647,
                    MaxBytesPerRead = 2147483647,
                    MaxNameTableCharCount = 2147483647
                },
                Security = new BasicHttpsSecurity
                {
                    Mode = BasicHttpsSecurityMode.TransportWithMessageCredential,
                    Transport = new HttpTransportSecurity
                    {
                        ClientCredentialType = HttpClientCredentialType.Basic
                    }
                }
            };
        }

        public BasicHttpsBinding GetBasicHttpsBindingStreamed()
        {
            return new BasicHttpsBinding
            {
                MaxBufferPoolSize = 2097120,
                MaxBufferSize = 65536,
                MaxReceivedMessageSize = 2147483647,
                CloseTimeout = new TimeSpan(0, 25, 0),
                OpenTimeout = new TimeSpan(0, 25, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                TransferMode = TransferMode.StreamedRequest,
                Security = new BasicHttpsSecurity
                {
                    Mode = BasicHttpsSecurityMode.TransportWithMessageCredential,
                    Transport = new HttpTransportSecurity
                    {
                        ClientCredentialType = HttpClientCredentialType.Basic
                    }
                }
            };
        }

        public BasicHttpsBinding GetBasicHttpsBindingCertificateAuthentication()
        {
            return new BasicHttpsBinding
            {
                Security = new BasicHttpsSecurity
                {
                    Mode = BasicHttpsSecurityMode.Transport,
                    Transport = new HttpTransportSecurity
                    {
                        ClientCredentialType = HttpClientCredentialType.Certificate
                    }
                }
            };
        }
    }
}