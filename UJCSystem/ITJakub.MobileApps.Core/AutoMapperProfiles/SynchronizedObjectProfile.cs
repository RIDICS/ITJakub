using AutoMapper;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class SynchronizedObjectProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<SynchronizedObjectContract, SynchronizedObject>().ReverseMap();
        }
    }
}