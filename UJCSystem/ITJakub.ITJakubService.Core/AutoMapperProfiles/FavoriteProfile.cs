using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class FavoriteProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<FavoriteBase, FavoriteBaseInfoContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.FavoriteType, opts => opts.MapFrom(src => src.FavoriteTypeEnum))
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => src.CreateTime));

            CreateMap<FavoriteBase, FavoriteBaseDetailContract>()
                .ForMember(dest => dest.FavoriteLabel, opts => opts.MapFrom(src => src.FavoriteLabel));


            CreateMap<FavoriteTypeEnum, FavoriteTypeContract>()
                .ReverseMap();
        }
    }
}
