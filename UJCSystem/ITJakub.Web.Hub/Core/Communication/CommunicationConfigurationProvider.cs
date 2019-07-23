using System;
using System.ServiceModel;
using System.Xml;
using Microsoft.Extensions.Options;
using Vokabular.Shared.Options;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationConfigurationProvider : CommunicationConfigurationProviderBase
    {
        public CommunicationConfigurationProvider(IOptions<EndpointOption> endpointOptions) : base(endpointOptions)
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
            var binding = new BasicHttpsBinding
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
            };

            binding.Security.Mode = BasicHttpsSecurityMode.TransportWithMessageCredential;
            binding.Security.Transport = new HttpTransportSecurity
            {
                ClientCredentialType = HttpClientCredentialType.Basic
            };

            return binding;
        }

        public BasicHttpsBinding GetBasicHttpsBindingStreamed()
        {
            var binding = new BasicHttpsBinding
            {
                MaxBufferPoolSize = 2097120,
                MaxBufferSize = 65536,
                MaxReceivedMessageSize = 2147483647,
                CloseTimeout = new TimeSpan(0, 25, 0),
                OpenTimeout = new TimeSpan(0, 25, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                TransferMode = TransferMode.StreamedRequest,
            };

            binding.Security.Mode = BasicHttpsSecurityMode.TransportWithMessageCredential;
            binding.Security.Transport = new HttpTransportSecurity
            {
                ClientCredentialType = HttpClientCredentialType.Basic
            };

            return binding;
        }

        public BasicHttpsBinding GetBasicHttpsBindingCertificateAuthentication()
        {
            var binding = new BasicHttpsBinding();

            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            binding.Security.Transport = new HttpTransportSecurity
            {
                ClientCredentialType = HttpClientCredentialType.Certificate
            };

            return binding;
        }
    }
}