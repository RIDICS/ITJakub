using System.Linq;
using AutoMapper;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.MainService.DataContracts.Contracts.CardFile;

namespace Vokabular.MainService.Core.AutoMapperProfiles.CardFile
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