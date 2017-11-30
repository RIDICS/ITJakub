using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.AutoMapperProfiles
{
    public class SpecialPermissionProfile : Profile
    {
        public SpecialPermissionProfile()
        {
            CreateMap<SpecialPermission, SpecialPermissionContract>()
                .Include<UploadBookPermission, UploadBookPermissionContract>()
                .Include<NewsPermission, NewsPermissionContract>()
                .Include<ManagePermissionsPermission, ManagePermissionsPermissionContract>()
                .Include<FeedbackPermission, FeedbackPermissionContract>()
                .Include<CardFilePermission, CardFilePermissionContract>()
                .Include<AutoImportCategoryPermission, AutoImportCategoryPermissionContract>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id));

            CreateMap<UploadBookPermission, UploadBookPermissionContract>()
                .ForMember(dest => dest.CanUploadBook, opts => opts.MapFrom(src => src.CanUploadBook));

            CreateMap<NewsPermission, NewsPermissionContract>()
                .ForMember(dest => dest.CanAddNews, opts => opts.MapFrom(src => src.CanAddNews));

            CreateMap<ManagePermissionsPermission, ManagePermissionsPermissionContract>()
                .ForMember(dest => dest.CanManagePermissions, opts => opts.MapFrom(src => src.CanManagePermissions));

            CreateMap<FeedbackPermission, FeedbackPermissionContract>()
                .ForMember(dest => dest.CanManageFeedbacks, opts => opts.MapFrom(src => src.CanManageFeedbacks));

            CreateMap<CardFilePermission, CardFilePermissionContract>()
                .ForMember(dest => dest.CanReadCardFile, opts => opts.MapFrom(src => src.CanReadCardFile))
                .ForMember(dest => dest.CardFileId, opts => opts.MapFrom(src => src.CardFileId))
                .ForMember(dest => dest.CardFileName, opts => opts.MapFrom(src => src.CardFileName));

            CreateMap<AutoImportCategoryPermission, AutoImportCategoryPermissionContract>()
                .ForMember(dest => dest.AutoImportIsAllowed, opts => opts.MapFrom(src => src.AutoImportIsAllowed))
                .ForMember(dest => dest.Category, opts => opts.MapFrom(src => src.Category));

            CreateMap<ReadLemmatizationPermission, ReadLemmatizationPermissionContract>()
                .ForMember(dest => dest.CanReadLemmatization, opts => opts.MapFrom(src => src.CanReadLemmatization));

            CreateMap<EditLemmatizationPermission, EditLemmatizationPermissionContract>()
                .ForMember(dest => dest.CanEditLemmatization, opts => opts.MapFrom(src => src.CanEditLemmatization));

            CreateMap<DerivateLemmatizationPermission, DerivateLemmatizationPermissionContract>()
                .ForMember(dest => dest.CanDerivateLemmatization, opts => opts.MapFrom(src => src.CanDerivateLemmatization));

            CreateMap<EditionPrintTextPermission, EditionPrintPermissionContract>()
                .ForMember(dest => dest.CanEditionPrintText, opts => opts.MapFrom(src => src.CanEditionPrintText));

            CreateMap<EditStaticTextPermission, EditStaticTextPermissionContract>()
                .ForMember(dest => dest.CanEditStaticText, opts => opts.MapFrom(src => src.CanEditStaticText));
        }
    }
}