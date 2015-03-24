using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class CategoryProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Category, CategoryContract>();
        }
    }
}