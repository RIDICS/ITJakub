using System.Collections.Generic;
using AutoMapper;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BucketContractProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<bucket, BucketContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.name))
                .ForMember(dest => dest.CardsCount, opts => opts.MapFrom(src => src.cards.count))
                .ForMember(dest => dest.Cards, opts => opts.MapFrom(src => Mapper.Map<card[], IList<CardContract>>(src.cards.card)));
        }
    }
}