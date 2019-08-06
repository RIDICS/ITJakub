using ITJakub.FileProcessing.DataContracts;
using ITJakub.SearchService.DataContracts;
using Ridics.Authentication.HttpClient.Client.Auth;
using Vokabular.CardFile.Core;
using Vokabular.FulltextService.DataContracts.Clients;

namespace Vokabular.MainService.Core.Communication
{
    public class CommunicationProvider
    {
        private readonly CommunicationConfigurationProvider m_configurationProvider;
        private readonly UserApiClient m_userApiClient;
        private readonly RoleApiClient m_roleApiClient;
        private readonly PermissionApiClient m_permissionApiClient;
        private readonly RegistrationApiClient m_registrationApiClient;
        private readonly ContactApiClient m_contactApiClient;
        private readonly FulltextServiceClient m_fulltextServiceClient;
        private readonly CardFilesClient m_cardFilesClient;

        private const string FileProcessingServiceEndpointName = "FileProcessingService";
        private const string SearchServiceEndpointName = "SearchService";

        public CommunicationProvider(CommunicationConfigurationProvider communicationConfigurationProvider, UserApiClient userApiClient,
            RoleApiClient roleApiClient, PermissionApiClient permissionApiClient, RegistrationApiClient registrationApiClient,
            ContactApiClient contactApiClient, FulltextServiceClient fulltextServiceClient, CardFilesClient cardFilesClient)
        {
            m_configurationProvider = communicationConfigurationProvider;
            m_userApiClient = userApiClient;
            m_roleApiClient = roleApiClient;
            m_permissionApiClient = permissionApiClient;
            m_registrationApiClient = registrationApiClient;
            m_contactApiClient = contactApiClient;
            m_fulltextServiceClient = fulltextServiceClient;
            m_cardFilesClient = cardFilesClient;
        }

        public FileProcessingServiceClient GetFileProcessingClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(FileProcessingServiceEndpointName);
            var binding = m_configurationProvider.GetBasicHttpBindingStreamed();
            var client = new FileProcessingServiceClient(binding, endpoint);
            return client;
        }

        public FulltextServiceClient GetFulltextServiceClient()
        {
            return m_fulltextServiceClient;
        }

        public SearchServiceClient GetSearchServiceClient()
        {
            var endpoint = m_configurationProvider.GetEndpointAddress(SearchServiceEndpointName);
            var binding = m_configurationProvider.GetBasicHttpBinding();
            var client = new SearchServiceClient(binding, endpoint);
            return client;
        }

        public CardFilesClient GetCardFilesClient()
        {
            return m_cardFilesClient;
        }

        public UserApiClient GetAuthUserApiClient()
        {
            return m_userApiClient;
        }

        public RoleApiClient GetAuthRoleApiClient()
        {
            return m_roleApiClient;
        }

        public PermissionApiClient GetAuthPermissionApiClient()
        {
            return m_permissionApiClient;
        }

        public RegistrationApiClient GetAuthRegistrationApiClient()
        {
            return m_registrationApiClient;
        }

        public ContactApiClient GetAuthContactApiClient()
        {
            return m_contactApiClient;
        }
    }
}