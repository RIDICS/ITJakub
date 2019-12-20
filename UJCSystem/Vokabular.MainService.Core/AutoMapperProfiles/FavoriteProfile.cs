﻿using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.Shared.DataContracts.Types.Favorite;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class FavoriteProfile : Profile
    {
        public FavoriteProfile()
        {
            // General types

            CreateMap<FavoriteBase, FavoriteBaseInfoContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.FavoriteType, opt => opt.MapFrom(src => src.FavoriteTypeEnum))
                .ForMember(dest => dest.FavoriteLabelId, opt => opt.MapFrom(src => src.FavoriteLabel.Id))
                .Include<FavoriteQuery, FavoriteBaseInfoContract>()
                .Include<FavoritePage, FavoriteBaseInfoContract>();

            CreateMap<FavoriteBase, FavoriteBaseWithLabelContract>()
                .IncludeBase<FavoriteBase, FavoriteBaseInfoContract>()
                .ForMember(dest => dest.FavoriteLabel, opt => opt.MapFrom(src => src.FavoriteLabel));

            CreateMap<FavoriteBase, FavoriteFullInfoContract>()
                .IncludeBase<FavoriteBase, FavoriteBaseInfoContract>();

            // Specific type to general

            CreateMap<FavoriteQuery, FavoriteBaseInfoContract>();
            CreateMap<FavoritePage, FavoriteBaseInfoContract>();

            // Specific types

            CreateMap<FavoriteQuery, FavoriteQueryContract>()
                .IncludeBase<FavoriteBase, FavoriteBaseInfoContract>()
                .ForMember(dest => dest.BookType, opt => opt.MapFrom(src => src.BookType.Type))
                .ForMember(dest => dest.FavoriteLabel, opt => opt.MapFrom(src => src.FavoriteLabel))
                .ForMember(dest => dest.Query, opt => opt.MapFrom(src => src.Query))
                .ForMember(dest => dest.QueryType, opt => opt.MapFrom(src => src.QueryType));

            CreateMap<FavoritePage, FavoritePageContract>()
                .IncludeBase<FavoriteBase, FavoriteBaseInfoContract>()
                .ForMember(dest => dest.FavoriteLabel, opt => opt.MapFrom(src => src.FavoriteLabel))
                .ForMember(dest => dest.PageId, opt => opt.MapFrom(src => src.ResourcePage.Id));
            
            CreateMap<FavoriteHeadword, FavoriteHeadwordContract>()
                .IncludeBase<FavoriteBase, FavoriteBaseInfoContract>()
                .ForMember(dest => dest.Query, opt => opt.MapFrom(src => src.Query))
                .ForMember(dest => dest.HeadwordId, opt => opt.MapFrom(src => src.DefaultHeadwordResource.Id));

            // Enum

            CreateMap<FavoriteTypeEnum, FavoriteTypeEnumContract>().ReverseMap();
        }
    }
}