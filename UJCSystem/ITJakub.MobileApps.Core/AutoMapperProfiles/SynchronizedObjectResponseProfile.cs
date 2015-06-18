using AutoMapper;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class SynchronizedObjectResponseProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<SynchronizedObjectBase, SynchronizedObjectResponseContract>()
                .Include<SynchronizedObject, SynchronizedObjectResponseContract>()
                .Include<SingleSynchronizedObject, SynchronizedObjectResponseContract>(); //TODO check reverseMap()

            CreateMap<SynchronizedObject, SynchronizedObjectResponseContract>();
            CreateMap<SingleSynchronizedObject, SynchronizedObjectResponseContract>().ForMember(x => x.Data, opt => opt.MapFrom(src => src.ObjectValue));
        }
    }
}
