using AutoMapper;
using ITJakub.Lemmatization.DataEntities.Entities;
using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Lemmatization.Core.AutoMapperProfiles
{
    public class HyperCanonicalFormProfile : Profile
    {
        public HyperCanonicalFormProfile()
        {
            CreateMap<HyperCanonicalForm, HyperCanonicalFormContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.Text))
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type));

            CreateMap<HyperCanonicalFormType, HyperCanonicalFormTypeContract>()
                .ReverseMap();
        }
    }
}