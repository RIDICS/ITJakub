using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class BookContentItemProfile : Profile
    {
        public BookContentItemProfile()
        {
            CreateMap<BookContentItem, BookContentItemContract>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.ReferredPageName, opt => opt.MapFrom(src => src.Page.Text))
                .ForMember(dest => dest.ReferredPageXmlId, opt => opt.MapFrom(src => src.Page.XmlId))
                .ForMember(dest => dest.ChildBookContentItems, opt => opt.MapFrom(src => src.ChildContentItems))
                .ForAllMembers(opt => opt.Condition(src => src != null));
        }
    }
}