using System;
using Vokabular.MainService.DataContracts;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class UriProvider : IMainServiceUriProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private const string MainServiceEndpointName = "MainService";

        public UriProvider(CommunicationConfigurationProvider communicationConfigurationProvider)
        {
            m_configurationProvider = communicationConfigurationProvider;
            MainServiceUri = m_configurationProvider.GetEndpointUri(MainServiceEndpointName);
        }

        public Uri MainServiceUri { get; set; }
    }
}
