using System.Linq;
using AutoMapper;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class CardShortContractProfile : Profile
    {
        public CardShortContractProfile()
        {
            CreateMap<card, CardShortContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id))
                .ForMember(dest => dest.Position, opts => opts.MapFrom(src => src.position))
                .ForMember(dest => dest.Headwords, opts => opts.MapFrom(src => src.Fields.Where(x => x.id == "heslo").Select(x => x.value)))    
                .ForSourceMember(source => source.image, opt => opt.Ignore())
                .ForSourceMember(source => source.note, opt => opt.Ignore())
                .ForSourceMember(source => source.warning, opt => opt.Ignore());   
        }
    }
}