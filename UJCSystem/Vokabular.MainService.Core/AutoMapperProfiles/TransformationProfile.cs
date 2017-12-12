using AutoMapper;
using ITJakub.SearchService.DataContracts.Types;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers.Fulltext.Data;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class TransformationProfile : Profile
    {
        public TransformationProfile()
        {
            CreateMap<ResourceLevelEnum, ResourceLevelEnumContract>().ReverseMap();

            CreateMap<OutputFormatEnum, OutputFormatEnumContract>().ReverseMap();

            CreateMap<Transformation, TransformationData>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.OutputFormat, opt => opt.MapFrom(src => src.OutputFormat))
                .ForMember(dest => dest.ResourceLevel, opt => opt.MapFrom(src => src.ResourceLevel));
        }
    }
}