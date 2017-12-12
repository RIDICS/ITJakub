using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<NewsSyndicationItem, NewsSyndicationItemContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));

            CreateMap<SyndicationItemType, NewsTypeEnumContract>().ReverseMap();
        }
    }
}