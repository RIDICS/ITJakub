using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
