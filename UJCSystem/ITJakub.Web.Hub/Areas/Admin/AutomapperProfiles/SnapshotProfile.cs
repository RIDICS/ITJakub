using System.Linq;
using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.Models;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles
{
    public class SnapshotProfile : Profile
    {
        public SnapshotProfile()
        {
            CreateMap<SnapshotAggregatedInfoContract, SnapshotViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.PublishedAudioResourceCount,
                    opt => opt.MapFrom(src => src.ResourcesInfo.FirstOrDefault(x => x.ResourceType == ResourceTypeEnumContract.Audio).PublishedCount))
                .ForMember(dest => dest.PublishedImageResourceCount,
                    opt => opt.MapFrom(src => src.ResourcesInfo.FirstOrDefault(x => x.ResourceType == ResourceTypeEnumContract.Image).PublishedCount))
                .ForMember(dest => dest.PublishedTextResourceCount,
                    opt => opt.MapFrom(src => src.ResourcesInfo.FirstOrDefault(x => x.ResourceType == ResourceTypeEnumContract.Text).PublishedCount));
        }
    }
}