using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class SpecialPermissionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<UploadBookPermission, UploadBookPermissionContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.CanUploadBook, opts => opts.MapFrom(src => src.CanUploadBook));

            CreateMap<NewsPermission, NewsPermissionContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.CanAddNews, opts => opts.MapFrom(src => src.CanAddNews));

            CreateMap<ManagePermissionsPermission, ManagePermissionsPermissionContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.CanManagePermissions, opts => opts.MapFrom(src => src.CanManagePermissions));

            CreateMap<FeedbackPermission, FeedbackPermissionContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.CanManageFeedbacks, opts => opts.MapFrom(src => src.CanManageFeedbacks));
        }
    }
}