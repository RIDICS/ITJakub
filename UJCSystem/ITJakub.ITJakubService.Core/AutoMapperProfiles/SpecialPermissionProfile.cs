using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class SpecialPermissionProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<SpecialPermission, SpecialPermissionContract>()
                .Include<UploadBookPermission, UploadBookPermissionContract>()
                .Include<NewsPermission, NewsPermissionContract>()
                .Include<ManagePermissionsPermission, ManagePermissionsPermissionContract>()
                .Include<FeedbackPermission, FeedbackPermissionContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id));

            CreateMap<UploadBookPermission, UploadBookPermissionContract>()
                .ForMember(dest => dest.CanUploadBook, opts => opts.MapFrom(src => src.CanUploadBook));

            CreateMap<NewsPermission, NewsPermissionContract>()
                .ForMember(dest => dest.CanAddNews, opts => opts.MapFrom(src => src.CanAddNews));

            CreateMap<ManagePermissionsPermission, ManagePermissionsPermissionContract>()
                .ForMember(dest => dest.CanManagePermissions, opts => opts.MapFrom(src => src.CanManagePermissions));

            CreateMap<FeedbackPermission, FeedbackPermissionContract>()
                .ForMember(dest => dest.CanManageFeedbacks, opts => opts.MapFrom(src => src.CanManageFeedbacks));
        }
    }
}