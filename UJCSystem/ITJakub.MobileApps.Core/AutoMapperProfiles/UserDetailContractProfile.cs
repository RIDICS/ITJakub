using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class UserDetailContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<UserDetailContract, User>()
                .Include<PasswordUserDetailContract, User>();

            Mapper.CreateMap<PasswordUserDetailContract, User>();
        }
    }
}