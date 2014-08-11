using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<UserDetailContract, User>()
                .Include<PasswordUserDetailContract, User>()
                .Include<GroupMemberContract, User>();

            CreateMap<PasswordUserDetailContract, User>();
            CreateMap<GroupMemberContract, User>().ReverseMap();
        }
    }
}