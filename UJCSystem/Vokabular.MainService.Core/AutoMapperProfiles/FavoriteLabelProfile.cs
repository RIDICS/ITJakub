using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Favorite;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class FavoriteLabelProfile : Profile
    {
        public FavoriteLabelProfile()
        {
            CreateMap<FavoriteLabel, FavoriteLabelContractBase>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color));

            CreateMap<FavoriteLabel, FavoriteLabelContract>()
                .IncludeBase<FavoriteLabel, FavoriteLabelContractBase>()
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.LastUseTime, opt => opt.MapFrom(src => src.LastUseTime));

            CreateMap<FavoriteLabel, FavoriteLabelWithBooksAndCategories>()
                .IncludeBase<FavoriteLabel, FavoriteLabelContract>();
        }
    }
}