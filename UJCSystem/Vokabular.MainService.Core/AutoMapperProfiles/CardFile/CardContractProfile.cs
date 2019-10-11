using System.Linq;
using AutoMapper;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.MainService.DataContracts.Contracts.CardFile;

namespace Vokabular.MainService.Core.AutoMapperProfiles.CardFile
{
    public class CardContractProfile : Profile
    {
        public CardContractProfile()
        {
            CreateMap<card, CardContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id))
                .ForMember(dest => dest.Position, opts => opts.MapFrom(src => src.position))
                .ForMember(dest => dest.Headwords, opts => opts.MapFrom(src => src.Fields.Where(x => x.id == "heslo").Select(x => x.value)))    
                .ForMember(dest => dest.Images, opts => opts.MapFrom(src => src.image))
                .ForMember(dest => dest.Notes, opts => opts.MapFrom(src => src.Fields.Where(x => x.id == "comment").Select(x => x.value)))
                .ForMember(dest => dest.Warnings, opts => opts.MapFrom(src => src.Fields.Where(x => x.id == "warning").Select(x => x.value))) 
                .ForSourceMember(source => source.headword, opt => opt.DoNotValidate())
                .ForSourceMember(source => source.note, opt => opt.DoNotValidate())
                .ForSourceMember(source => source.warning, opt => opt.DoNotValidate());   
        }
    }
}