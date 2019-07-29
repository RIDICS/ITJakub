using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class FilteringExpressionSetProfile : Profile
    {
        public FilteringExpressionSetProfile()
        {
            CreateMap<FilteringExpressionSet, FilteringExpressionSetContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BibliographicFormat, opt => opt.MapFrom(src => src.BibliographicFormat));

            CreateMap<FilteringExpressionSet, FilteringExpressionSetDetailContract>()
                .IncludeBase<FilteringExpressionSet, FilteringExpressionSetContract>()
                .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser))
                .ForMember(dest => dest.FilteringExpressions, opt => opt.MapFrom(src => src.FilteringExpressions));
        }
    }
}