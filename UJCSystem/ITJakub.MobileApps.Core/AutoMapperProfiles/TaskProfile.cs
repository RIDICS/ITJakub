using AutoMapper;
using ITJakub.MobileApps.DataContracts.Tasks;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class TaskProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Task, TaskContract>()
                .ForMember(taskContract => taskContract.ApplicationId,
                    expression => expression.MapFrom(task => task.Application.Id))
                .Include<Task, TaskDataContract>()
                .Include<Task, TaskDetailContract>();

            CreateMap<Task, TaskDetailContract>();
            CreateMap<Task, TaskDataContract>();
        }
    }
}