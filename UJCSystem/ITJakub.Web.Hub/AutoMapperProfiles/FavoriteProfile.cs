using AutoMapper;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
using ITJakub.Web.Hub.Models.Favorite;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class FavoriteProfile : Profile
    {
        public FavoriteProfile()
        {
            CreateMap<FavoriteLabelContract, FavoriteLabelViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.LastUseTime, opt => opt.MapFrom(src => src.LastUseTime));

            CreateMap<FavoriteQueryContract, FavoriteQueryViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.FavoriteLabel, opt => opt.MapFrom(src => src.FavoriteLabel))
                .ForMember(dest => dest.Query, opt => opt.MapFrom(src => src.Query));
        }
    }
}