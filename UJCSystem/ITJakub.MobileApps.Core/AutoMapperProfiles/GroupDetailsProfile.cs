using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class GroupDetailsProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DE.Group, GroupDetail>()
                .Include<DE.Group, GroupDetail>()
                .ForMember(
                    gd => gd.GroupName,
                    opt => opt.MapFrom(s => s.Name)
                );

            CreateMap<DE.Group, OwnedGroupDetail>()
                .ForMember(
                    gd => gd.GroupAccessCode,
                    opt => opt.MapFrom(s => s.EnterCode)
                );
        }
    }
}