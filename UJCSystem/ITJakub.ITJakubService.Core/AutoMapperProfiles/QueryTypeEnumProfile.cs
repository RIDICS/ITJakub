using AutoMapper;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.Shared.Contracts.Favorites;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class QueryTypeEnumProfile : Profile
    {
        public QueryTypeEnumProfile()
        {
            CreateMap<QueryTypeEnum, QueryTypeEnumContract>().ReverseMap();
        }
    }
}