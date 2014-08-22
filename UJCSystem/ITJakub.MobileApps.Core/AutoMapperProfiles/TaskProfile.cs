using AutoMapper;
using ITJakub.MobileApps.DataContracts.Tasks;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class TaskProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Task, TaskContract>();
        }
    }
}