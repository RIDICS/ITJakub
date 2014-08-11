using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class UserDetailContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Group, GroupDetailContract>()
                .Include<Group, OwnedDetailGroupContract>();

            CreateMap<Group, OwnedDetailGroupContract>();
        }
    }
}