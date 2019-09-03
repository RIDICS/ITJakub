using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class FilteringExpressionProfile : Profile
    {
        public FilteringExpressionProfile()
        {
            CreateMap<FilteringExpression, FilteringExpressionContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.Field))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
        }
    }
}