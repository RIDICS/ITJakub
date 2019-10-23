using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class BasicBookProfile : Profile
    {
        public BasicBookProfile()
        {
            CreateMap<MetadataResource, BookWithCategoriesContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Project.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.SubTitle, opt => opt.MapFrom(src => src.SubTitle))
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.Resource.Project.Categories.Select(x => x.Id)));
        }
    }
}