using AutoMapper;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class GroupProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Group, GroupInfoContract>()
                .Include<Group, OwnedGroupInfoContract>();

            CreateMap<Group, OwnedGroupInfoContract>();
            CreateMap<Group, GroupDetailContract>();

            CreateMap<Group, GroupDetailsUpdateContract>();
        }
    }
}