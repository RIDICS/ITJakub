using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class TaskProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Task, DE.Task>().ReverseMap();
        }
    }
}