using AutoMapper;
using ITJakub.Web.Hub.Options;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class PortalTypeProfile : Profile
    {
        public PortalTypeProfile()
        {
            CreateMap<PortalType, PortalTypeContract>();
        }
    }
}