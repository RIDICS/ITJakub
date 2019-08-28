using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class SnapshotProfile : Profile
    {
        public SnapshotProfile()
        {
            CreateMap<Snapshot, SnapshotContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookTypes, opt => opt.MapFrom(src => src.BookTypes.Select(x => x.Type)))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedByUser.Id))
                .ForMember(dest => dest.DefaultBookType, opt => opt.MapFrom(src => src.DefaultBookType.Type))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.Id))
                .ForMember(dest => dest.PublishTime, opt => opt.MapFrom(src => src.PublishTime))
                .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.VersionNumber));
        }
    }
}