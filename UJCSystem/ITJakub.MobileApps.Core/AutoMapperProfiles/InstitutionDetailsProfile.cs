using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class InstitutionDetailsProfile : Profile
    {
        protected override void Configure()
        {
            RecognizeAlias("Members", "Employees");
            CreateMap<DE.Institution, InstitutionDetails>();
        }
    }
}
