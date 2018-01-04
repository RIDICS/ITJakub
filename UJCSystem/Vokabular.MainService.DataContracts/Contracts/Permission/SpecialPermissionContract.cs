namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    public class SpecialPermissionContract
    {
        public int Id { get; set; }
    }

    public class NewsPermissionContract : SpecialPermissionContract
    {
        public bool CanAddNews { get; set; }
    }

    public class UploadBookPermissionContract : SpecialPermissionContract
    {
        public bool CanUploadBook { get; set; }
    }

    public class ManagePermissionsPermissionContract : SpecialPermissionContract
    {
        public bool CanManagePermissions { get; set; }
    }

    public class FeedbackPermissionContract : SpecialPermissionContract
    {
        public bool CanManageFeedbacks { get; set; }
    }

    public class CardFilePermissionContract : SpecialPermissionContract
    {
        public bool CanReadCardFile { get; set; }

        public string CardFileId { get; set; }

        public string CardFileName { get; set; }
    }
    
    public class AutoImportCategoryPermissionContract : SpecialPermissionContract
    {
        public bool AutoImportIsAllowed { get; set; }
        
        public CategoryContract Category { get; set; }
    }

    public class ReadLemmatizationPermissionContract : SpecialPermissionContract
    {
        public bool CanReadLemmatization { get; set; }
    }

    public class EditLemmatizationPermissionContract : SpecialPermissionContract
    {
        public bool CanEditLemmatization { get; set; }
    }

    public class DerivateLemmatizationPermissionContract : SpecialPermissionContract
    {
        public bool CanDerivateLemmatization { get; set; }
    }
    
    public class EditionPrintPermissionContract : SpecialPermissionContract
    {
        public bool CanEditionPrintText { get; set; }
    }

    public class EditStaticTextPermissionContract : SpecialPermissionContract
    {
        public bool CanEditStaticText { get; set; }
    }
}