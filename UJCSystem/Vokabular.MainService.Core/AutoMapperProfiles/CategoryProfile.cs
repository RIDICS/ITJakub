using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategory.Id));

            CreateMap<Category, CategoryTreeContract>()
                .IncludeBase<Category, CategoryContract>()
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Categories));
        }
    }
}