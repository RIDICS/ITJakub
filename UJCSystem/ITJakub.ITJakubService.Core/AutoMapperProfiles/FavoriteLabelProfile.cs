using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class FavoriteLabelProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<FavoriteLabel, FavoriteLabelContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Color))
                .ForMember(dest => dest.IsDefault, opts => opts.MapFrom(src => src.IsDefault))
                .ForMember(dest => dest.LastUseTime, opts => opts.MapFrom(src => src.LastUseTime));
        }
    }
}