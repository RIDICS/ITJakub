using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class SynchronizedObjectDetailsProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DE.SynchronizedObject, SynchronizedObjectDetails>().ForMember(syncobj => syncobj.SynchronizedObject, opt => opt.MapFrom(s => Mapper.Map<SynchronizedObject>(s)));
        }
    }
}