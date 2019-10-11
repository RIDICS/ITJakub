using AutoMapper;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Options;
using Microsoft.Extensions.Options;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Core
{
    public class ControllerDataProvider
    {
        private readonly IOptions<PortalOption> m_portalOption;

        public ControllerDataProvider(CommunicationProvider communicationProvider, IOptions<PortalOption> portalOption, IMapper mapper,
            ILocalizationService localizationService)
        {
            m_portalOption = portalOption;
            CommunicationProvider = communicationProvider;
            Mapper = mapper;
            Localizer = localizationService;
        }
        
        public CommunicationProvider CommunicationProvider { get; }

        public PortalTypeContract PortalType => Mapper.Map<PortalTypeContract>(m_portalOption.Value.PortalType);

        public IMapper Mapper { get; }

        public ILocalizationService Localizer { get; }
    }
}