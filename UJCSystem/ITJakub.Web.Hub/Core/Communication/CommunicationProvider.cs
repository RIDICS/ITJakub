using System.ServiceModel;
using AutoMapper;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Options;
using Microsoft.Extensions.Options;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly MainServiceBookClient m_bookClient;
        private readonly MainServiceCardFileClient m_cardFileClient;
        private readonly MainServiceCodeListClient m_codeListClient;
        private readonly MainServiceExternalRepositoryClient m_externalRepositoryClient;
        private readonly MainServiceFavoriteClient m_favoriteClient;
        private readonly MainServiceFeedbackClient m_feedbackClient;
        private readonly MainServiceFilteringExpressionSetClient m_filteringExpressionSetClient;
        private readonly MainServiceMetadataClient m_metadataClient;
        private readonly MainServiceNewsClient m_newsClient;
        private readonly MainServicePermissionClient m_permissionClient;
        private readonly MainServiceProjectClient m_projectClient;
        private readonly MainServiceResourceClient m_resourceClient;
        private readonly MainServiceRoleClient m_roleClient;
        private readonly MainServiceSessionClient m_sessionClient;
        private readonly MainServiceSnapshotClient m_snapshotClient;
        private readonly MainServiceTermClient m_termClient;
        private readonly MainServiceUserClient m_userClient;
        private readonly IOptions<PortalOption> m_portalOption;

        private const string LemmatizationServiceEndpointName = "LemmatizationService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider,
            MainServiceBookClient bookClient, MainServiceCardFileClient cardFileClient, MainServiceCodeListClient codeListClient,
            MainServiceExternalRepositoryClient externalRepositoryClient, MainServiceFavoriteClient favoriteClient,
            MainServiceFeedbackClient feedbackClient, MainServiceFilteringExpressionSetClient filteringExpressionSetClient,
            MainServiceMetadataClient metadataClient, MainServiceNewsClient newsClient, MainServicePermissionClient permissionClient,
            MainServiceProjectClient projectClient, MainServiceResourceClient resourceClient, MainServiceRoleClient roleClient,
            MainServiceSessionClient sessionClient, MainServiceSnapshotClient snapshotClient, MainServiceTermClient termClient,
            MainServiceUserClient userClient, IOptions<PortalOption> portalOption)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_bookClient = bookClient;
            m_cardFileClient = cardFileClient;
            m_codeListClient = codeListClient;
            m_externalRepositoryClient = externalRepositoryClient;
            m_favoriteClient = favoriteClient;
            m_feedbackClient = feedbackClient;
            m_filteringExpressionSetClient = filteringExpressionSetClient;
            m_metadataClient = metadataClient;
            m_newsClient = newsClient;
            m_permissionClient = permissionClient;
            m_projectClient = projectClient;
            m_resourceClient = resourceClient;
            m_roleClient = roleClient;
            m_sessionClient = sessionClient;
            m_snapshotClient = snapshotClient;
            m_termClient = termClient;
            m_userClient = userClient;
            m_portalOption = portalOption;
        }

        public PortalTypeContract PortalType => Mapper.Map<PortalTypeContract>(m_portalOption.Value.PortalType);

        public MainServiceBookClient GetMainServiceBookClient()
        {
            return m_bookClient;
        }

        public MainServiceCardFileClient GetMainServiceCardFileClient()
        {
            return m_cardFileClient;
        }

        public MainServiceCodeListClient GetMainServiceCodeListClient()
        {
            return m_codeListClient;
        }

        public MainServiceExternalRepositoryClient GetMainServiceExternalRepositoryClient()
        {
            return m_externalRepositoryClient;
        }

        public MainServiceFavoriteClient GetMainServiceFavoriteClient()
        {
            return m_favoriteClient;
        }

        public MainServiceFeedbackClient GetMainServiceFeedbackClient()
        {
            return m_feedbackClient;
        }

        public MainServiceFilteringExpressionSetClient GetMainServiceFilteringExpressionSetClient()
        {
            return m_filteringExpressionSetClient;
        }

        public MainServiceMetadataClient GetMainServiceMetadataClient()
        {
            return m_metadataClient;
        }

        public MainServiceNewsClient GetMainServiceNewsClient()
        {
            return m_newsClient;
        }

        public MainServicePermissionClient GetMainServicePermissionClient()
        {
            return m_permissionClient;
        }

        public MainServiceProjectClient GetMainServiceProjectClient()
        {
            return m_projectClient;
        }

        public MainServiceResourceClient GetMainServiceResourceClient()
        {
            return m_resourceClient;
        }

        public MainServiceRoleClient GetMainServiceRoleClient()
        {
            return m_roleClient;
        }

        public MainServiceSessionClient GetMainServiceSessionClient()
        {
            return m_sessionClient;
        }

        public MainServiceSnapshotClient GetMainServiceSnapshotClient()
        {
            return m_snapshotClient;
        }

        public MainServiceTermClient GetMainServiceTermClient()
        {
            return m_termClient;
        }

        public MainServiceUserClient GetMainServiceUserClient()
        {
            return m_userClient;
        }

        public LemmatizationServiceClient GetLemmatizationClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(LemmatizationServiceEndpointName);
            var binding = new BasicHttpBinding();
            return new LemmatizationServiceClient(binding, endpoint);
        }
    }
}