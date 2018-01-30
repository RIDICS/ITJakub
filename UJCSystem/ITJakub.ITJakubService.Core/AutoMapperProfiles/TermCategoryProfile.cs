using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class TermCategoryProfile : Profile
    {
        public TermCategoryProfile()
        {
            CreateMap<TermCategory, TermCategoryContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.Terms, opts => opts.MapFrom(src => src.Terms));
        }
    }
}