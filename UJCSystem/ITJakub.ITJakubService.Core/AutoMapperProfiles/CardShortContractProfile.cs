using AutoMapper;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class CardShortContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<card, CardShortContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id))
                .ForMember(dest => dest.Position, opts => opts.MapFrom(src => src.position))
                .ForMember(dest => dest.Headword, opts => opts.MapFrom(src => src.headword))
                .ForSourceMember(source => source.image, opt => opt.Ignore())
                .ForSourceMember(source => source.note, opt => opt.Ignore())
                .ForSourceMember(source => source.warning, opt => opt.Ignore());   
        }
    }
}