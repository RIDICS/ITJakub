using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Xml;
using Microsoft.Extensions.Options;
using Vokabular.Shared.Options;

namespace ITJakub.Web.Hub.Managers
{
    public class CommunicationConfigurationProvider : CommunicationConfigurationProviderBase
    {
        public CommunicationConfigurationProvider(IOptions<List<EndpointOption>> endpointOptions) : base(endpointOptions)
        {
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
                ReaderQuotas = new XmlDictionaryReaderQuotas
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
                ReaderQuotas = new XmlDictionaryReaderQuotas
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