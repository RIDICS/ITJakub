using AutoMapper;
using ITJakub.Web.DataEntities.Database.Entities.Enums;
using ITJakub.Web.Hub.Models.Type;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class StaticTextProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<StaticTextFormatType, StaticTextFormat>().ReverseMap();
        }
    }
}