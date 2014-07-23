using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class GroupDetailsProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DE.Group, GroupDetails>().ForMember(group => group.Group, opt => opt.MapFrom(s => Mapper.Map<Group>(s)));
        }
    }
}