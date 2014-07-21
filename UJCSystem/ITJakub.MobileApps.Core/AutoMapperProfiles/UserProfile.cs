using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<User, DE.User>().ReverseMap();
        }
    }
}
