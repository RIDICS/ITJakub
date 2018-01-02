using System.Collections.Generic;
using AutoMapper;
using Vokabular.CardFile.Core.DataContractEntities;
using Vokabular.MainService.DataContracts.Contracts.CardFile;

namespace Vokabular.MainService.Core.Managers.CardFile
{
    public class BucketShortContractProfile : Profile
    {
        public BucketShortContractProfile()
        {
            CreateMap<bucket, BucketShortContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.name))
                .ForMember(dest => dest.CardsCount, opts => opts.MapFrom(src => src.cards.count))
                .ForMember(dest => dest.Cards, opts => opts.MapFrom(src => Mapper.Map<card[], IList<CardShortContract>>(src.cards.card)));
        }
    }
}