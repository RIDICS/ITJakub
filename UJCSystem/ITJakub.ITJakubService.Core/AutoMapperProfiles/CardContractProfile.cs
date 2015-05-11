using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class CardContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<card, CardContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id))
                .ForMember(dest => dest.Position, opts => opts.MapFrom(src => src.position))
                .ForMember(dest => dest.Headwords, opts => opts.MapFrom(src => src.Fields.Where(x => x.id == "heslo").Select(x => x.value)))    
                .ForMember(dest => dest.Images, opts => opts.MapFrom(src => Mapper.Map<image[], IList<ImageContract>>(src.image)))
                .ForMember(dest => dest.Notes, opts => opts.MapFrom(src => src.Fields.Where(x => x.id == "comment").Select(x => x.value)))
                .ForMember(dest => dest.Warnings, opts => opts.MapFrom(src => src.Fields.Where(x => x.id == "warning").Select(x => x.value))) 
                .ForSourceMember(source => source.headword, opt => opt.Ignore())
                .ForSourceMember(source => source.note, opt => opt.Ignore())
                .ForSourceMember(source => source.warning, opt => opt.Ignore());   
        }
    }
}