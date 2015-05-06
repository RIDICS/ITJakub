using System.Collections.Generic;
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
                .ForMember(dest => dest.Headword, opts => opts.MapFrom(src => src.headword))    //TODO consult what is accurate if headword element or field element with headword as id attribute value
                .ForMember(dest => dest.Images, opts => opts.MapFrom(src => Mapper.Map<image[], IList<ImageContract>>(src.image)))
                .ForMember(dest => dest.Note, opts => opts.MapFrom(src => src.note)) //TODO consult what is accurate if note element or field element with note as id attribute value
                .ForMember(dest => dest.Warning, opts => opts.MapFrom(src => src.warning)); //TODO consult what is accurate if warning element or field element with warning as id attribute value
        }
    }
}