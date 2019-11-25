using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class NamedResourceGroupProfile : Profile
    {
        public NamedResourceGroupProfile()
        {
            CreateMap<NamedResourceGroup, NamedResourceGroupContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TextType, opt => opt.MapFrom(src => src.TextType));
        }
    }
}