using AutoMapper;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using Scalesoft.Localization.AspNetCore;

namespace ITJakub.Web.Hub.Core
{
    public class ControllerDataProvider
    {
        public ControllerDataProvider(CommunicationProvider communicationProvider, IMapper mapper,
            ILocalizationService localizationService, PortalTypeManager portalTypeManager)
        {
            CommunicationProvider = communicationProvider;
            Mapper = mapper;
            Localizer = localizationService;
            PortalTypeManager = portalTypeManager;
        }

        public CommunicationProvider CommunicationProvider { get; }
        
        public IMapper Mapper { get; }

        public ILocalizationService Localizer { get; }

        public PortalTypeManager PortalTypeManager { get; }
    }
}