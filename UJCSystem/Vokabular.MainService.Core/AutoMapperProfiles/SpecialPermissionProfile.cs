using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.AutoMapperProfiles
{
    public class SpecialPermissionProfile : Profile
    {
        public SpecialPermissionProfile()
        {
            CreateMap<SpecialPermissionCategorization, SpecialPermissionCategorizationEnumContract>().ReverseMap();

            CreateMap<SpecialPermission, SpecialPermissionContract>()
                .Include<UploadBookPermission, UploadBookPermissionContract>()
                .Include<NewsPermission, NewsPermissionContract>()
                .Include<ManagePermissionsPermission, ManagePermissionsPermissionContract>()
                .Include<FeedbackPermission, FeedbackPermissionContract>()
                .Include<CardFilePermission, CardFilePermissionContract>()
                .Include<AutoImportBookTypePermission, AutoImportCategoryPermissionContract>()
                .Include<ReadLemmatizationPermission, ReadLemmatizationPermissionContract>()
                .Include<EditLemmatizationPermission, EditLemmatizationPermissionContract>()
                .Include<DerivateLemmatizationPermission, DerivateLemmatizationPermissionContract>()
                .Include<EditionPrintTextPermission, EditionPrintPermissionContract>()
                .Include<EditStaticTextPermission, EditStaticTextPermissionContract>()
                .Include<ManageRepositoryImportPermission, ManageRepositoryImportPermissionContract>()
                .Include<ReadExternalProjectPermission, ReadExternalProjectPermissionContract>()
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

            CreateMap<AutoImportBookTypePermission, AutoImportCategoryPermissionContract>()
                .ForMember(dest => dest.AutoImportIsAllowed, opts => opts.MapFrom(src => src.AutoImportIsAllowed))
                .ForMember(dest => dest.BookType, opts => opts.MapFrom(src => src.BookType.Type));

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

            CreateMap<ManageRepositoryImportPermission, ManageRepositoryImportPermissionContract>()
                .ForMember(dest => dest.CanManagerRepositoryImport, opts => opts.MapFrom(src => src.CanManageRepositoryImport));
            
            CreateMap<ReadExternalProjectPermission, ReadExternalProjectPermissionContract>()
                .ForMember(dest => dest.CanReadExternalProject, opts => opts.MapFrom(src => src.CanReadExternalProject));
        }
    }
}