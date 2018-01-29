using AutoMapper;
using ITJakub.Web.Hub.Models.Favorite;
using Vokabular.MainService.DataContracts.Contracts.Favorite;

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
        }
    }
}