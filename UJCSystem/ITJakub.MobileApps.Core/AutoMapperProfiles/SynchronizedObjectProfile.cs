using AutoMapper;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class SynchronizedObjectProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<SynchronizedObject, DataEntities.Database.Entities.SynchronizedObject>().ReverseMap();
        }
    }
}