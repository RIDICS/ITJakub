using System;
using AutoMapper;
using ITJakub.Web.Hub.Options;
using Microsoft.Extensions.Options;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Core.Managers
{
    public class PortalTypeManager
    {
        private readonly IOptions<PortalOption> m_portalOptions;
        private readonly IMapper m_mapper;

        public PortalTypeManager(IOptions<PortalOption> portalOptions, IMapper mapper)
        {
            m_portalOptions = portalOptions;
            m_mapper = mapper;
        }

        public PortalOption Options => m_portalOptions.Value;

        public PortalTypeContract GetPortalType()
        {
            return m_mapper.Map<PortalTypeContract>(m_portalOptions.Value.PortalType);
        }

        public ProjectTypeContract GetDefaultProjectType()
        {
            switch (GetPortalType())
            {
                case PortalTypeContract.Research:
                    return ProjectTypeContract.Research;
                case PortalTypeContract.Community:
                    return ProjectTypeContract.Community;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}