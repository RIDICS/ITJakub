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
    public class InstitutionDetailsProfile : Profile
    {
        protected override void Configure()
        {
            RecognizeAlias("Users", "Employees");
            CreateMap<DE.Institution, InstitutionDetails>();
        }
    }
}
