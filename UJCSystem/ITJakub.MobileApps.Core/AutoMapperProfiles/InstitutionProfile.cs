using AutoMapper;
using ITJakub.MobileApps.DataContracts;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.Core.AutoMapperProfiles
{
    public class InstitutionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Institution, DE.Institution>().ReverseMap();
        }
    }
}