using AutoMapper;
using ITJakub.Web.DataEntities.Database.Entities;
using ITJakub.Web.DataEntities.Database.Entities.Enums;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;

namespace ITJakub.Web.Hub.AutoMapperProfiles
{
    public class StaticTextProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<StaticTextFormatType, StaticTextFormat>().ReverseMap();

            CreateMap<StaticText, StaticTextViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Format))
                .ForMember(dest => dest.IsRecordExists, opt => opt.UseValue(true));
        }
    }
}