using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class GroupProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Group, DE.Group>().ReverseMap();
        }
    }
}