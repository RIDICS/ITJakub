using AutoMapper;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BookTypeEnumProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<BookTypeEnum, BookTypeEnumContract>().ReverseMap();
        }
    }
}