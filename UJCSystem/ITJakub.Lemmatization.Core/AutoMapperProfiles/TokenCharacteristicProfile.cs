using AutoMapper;
using ITJakub.Lemmatization.DataEntities;
using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Lemmatization.Core.AutoMapperProfiles
{
    public class TokenCharacteristicProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<TokenCharacteristic, TokenCharacteristicContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.MorphologicalCharacteristic, opts => opts.MapFrom(src => src.MorphologicalCharakteristic))
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
                .ForMember(dest => dest.CanonicalFormList, opts => opts.MapFrom(src => src.CanonicalForms));
        }
    }
}