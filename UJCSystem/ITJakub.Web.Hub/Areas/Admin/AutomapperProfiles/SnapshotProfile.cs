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
            CreateMap<SnapshotContract, SnapshotViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dest => dest.AudioResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Audio).TotalCount))
                .ForMember(dest => dest.PublishedAudioResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Audio).TotalCount))
                .ForMember(dest => dest.ImageResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Image).TotalCount))
                .ForMember(dest => dest.PublishedImageResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Image).TotalCount))
                .ForMember(dest => dest.TextResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Text).TotalCount))
                .ForMember(dest => dest.PublishedTextResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Text).TotalCount))
                .ForMember(dest => dest.VideoResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Video).TotalCount))
                .ForMember(dest => dest.PublishedVideoResourceCount, opt => opt.MapFrom(src => src.ResourcesInfo.First(x => x.ResourceType == ResourceTypeContract.Video).TotalCount));
        }
    }
}