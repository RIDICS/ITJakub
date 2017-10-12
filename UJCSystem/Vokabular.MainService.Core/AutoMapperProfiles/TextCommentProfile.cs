using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class TextCommentProfile : Profile
    {
        public TextCommentProfile()
        {
            CreateMap<TextComment, TextCommentContractBase>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.TextReferenceId, opt => opt.MapFrom(src => src.TextReferenceId));

            CreateMap<TextComment, GetTextCommentContract>()
                .IncludeBase<TextComment, TextCommentContractBase>()
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.TextResourceId, opt => opt.MapFrom(src => src.ResourceText.Id))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.CreatedByUser))
                .ForMember(dest => dest.TextComments, opt => opt.MapFrom(src => src.TextComments));
        }
    }
}