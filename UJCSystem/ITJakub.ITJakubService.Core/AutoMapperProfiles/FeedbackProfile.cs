using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts.Notes;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class FeedbackProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Feedback, FeedbackContract>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(m => m.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(m => m.FilledName, opt => opt.MapFrom(src => src.Name))
                .ForMember(m => m.FilledEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(m => m.User, opt => opt.MapFrom(src => src.User))
                .ForMember(m => m.FeedbackType, opt => opt.MapFrom(src => src.FeedbackType));

            CreateMap<BookHeadword, FeedbackHeadwordInfoContract>()
                .ForMember(m => m.HeadwordId, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Headword, opt => opt.MapFrom(src => src.Headword))
                .ForMember(m => m.DefaultHeadword, opt => opt.MapFrom(src => src.DefaultHeadword))
                .ForMember(m => m.DictionaryName, opt => opt.MapFrom(src => src.BookVersion.Title));
        }
    }
}