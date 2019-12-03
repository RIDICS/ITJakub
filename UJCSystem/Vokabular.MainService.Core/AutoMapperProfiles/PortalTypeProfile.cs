using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class PortalTypeProfile : Profile
    {
        public PortalTypeProfile()
        {
            CreateMap<PortalTypeEnum, PortalTypeContract>().ReverseMap();
        }
    }
}