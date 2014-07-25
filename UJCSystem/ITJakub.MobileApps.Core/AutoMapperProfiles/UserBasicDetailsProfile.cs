using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class UserBasicDetailsProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DE.User, UserBasicDetails>().ForMember(user => user.User, opt => opt.MapFrom(s => Mapper.Map<User>(s)));
        }
    }
}