using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class ChapterProfile : Profile
    {
        public ChapterProfile()
        {
            CreateMap<ChapterResource, ChapterContractBase>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Id))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.BeginningPageId, opt => opt.MapFrom(src => src.ResourceBeginningPage.Id));

            CreateMap<ChapterResource, GetChapterContract>()
                .IncludeBase<ChapterResource, ChapterContractBase>()
                .ForMember(dest => dest.ParentChapterId, opt => opt.MapFrom(src => src.ParentResource.Id));

            CreateMap<ChapterResource, ChapterHierarchyContract>()
                .IncludeBase<ChapterResource, ChapterContractBase>();
        }
    }
}