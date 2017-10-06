using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class TrackProfile : Profile
    {
        public TrackProfile()
        {
            CreateMap<TrackResource, TrackContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Id))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.ChapterId, opt => opt.MapFrom(src => src.ResourceChapter.Id))
                .ForMember(dest => dest.BeginningPageId, opt => opt.MapFrom(src => src.ResourceBeginningPage.Id));
        }
    }
}