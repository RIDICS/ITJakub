using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class ManuscriptProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<ManuscriptDescription, ManuscriptContract>();
        }
    }
}