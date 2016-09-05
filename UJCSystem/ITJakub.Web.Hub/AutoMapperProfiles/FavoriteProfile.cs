using AutoMapper;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
using ITJakub.Web.Hub.Models.Favorite;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class FavoriteProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<FavoriteLabelContract, FavoriteLabelViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.LastUseTime, opt => opt.MapFrom(src => src.LastUseTime));
        }
    }
}