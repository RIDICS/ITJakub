using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class TaskDetailsProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DE.Task, TaskDetails>().ForMember(task => task.Task, opt => opt.MapFrom(s => Mapper.Map<Task>(s)));
        }
    }
}