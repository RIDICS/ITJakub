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
                .ForMember(x => x.AuthorId, opt => opt.MapFrom(x => x.Author.Id))
                .Include<Group, OwnedGroupInfoContract>()
                .Include<Group, GroupDetailContract>();

            CreateMap<Group, OwnedGroupInfoContract>();
            CreateMap<Group, GroupDetailContract>();

            CreateMap<Group, GroupDetailsUpdateContract>();

            CreateMap<GroupStateContract, GroupState>()
                .ReverseMap();
        }
    }
}