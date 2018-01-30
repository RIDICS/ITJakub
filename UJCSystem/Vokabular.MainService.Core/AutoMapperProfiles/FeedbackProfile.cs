using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<Feedback, FeedbackContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.AuthorEmail))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.AuthorName))
                .ForMember(dest => dest.AuthorUser, opt => opt.MapFrom(src => src.AuthorUser))
                .ForMember(dest => dest.FeedbackCategory, opt => opt.MapFrom(src => src.FeedbackCategory))
                .ForMember(dest => dest.FeedbackType, opt => opt.MapFrom(src => src.FeedbackType))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .Include<HeadwordFeedback, FeedbackContract>();

            CreateMap<HeadwordFeedback, FeedbackContract>()
                .ForMember(dest => dest.HeadwordInfo, opt => opt.MapFrom(src => src.HeadwordResource))
                .ForMember(dest => dest.ProjectInfo, opt => opt.MapFrom(src => src.HeadwordResource.Resource.Project));


            CreateMap<FeedbackCategoryEnum, FeedbackCategoryEnumContract>().ReverseMap();

            CreateMap<FeedbackSortEnum, FeedbackSortEnumContract>().ReverseMap();
        }
    }
}