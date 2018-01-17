using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Utils;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    [JsonConverter(typeof(SpecialPermissionJsonConverter))]
    public abstract class SpecialPermissionContract
    {
        public abstract SpecialPermissionTypeContract Key { get; }

        public int Id { get; set; }
    }

    public class NewsPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.News;

        public bool CanAddNews { get; set; }
    }

    public class UploadBookPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.UploadBook;

        public bool CanUploadBook { get; set; }
    }

    public class ManagePermissionsPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.Permissions;

        public bool CanManagePermissions { get; set; }
    }

    public class FeedbackPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.Feedback;

        public bool CanManageFeedbacks { get; set; }
    }

    public class CardFilePermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.CardFile;

        public bool CanReadCardFile { get; set; }

        public string CardFileId { get; set; }

        public string CardFileName { get; set; }
    }
    
    public class AutoImportCategoryPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.AutoImport;

        public bool AutoImportIsAllowed { get; set; }
        
        public BookTypeEnumContract BookType { get; set; }
    }

    public class ReadLemmatizationPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.ReadLemmatization;

        public bool CanReadLemmatization { get; set; }
    }

    public class EditLemmatizationPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.EditLemmatization;

        public bool CanEditLemmatization { get; set; }
    }

    public class DerivateLemmatizationPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.DerivateLemmatization;

        public bool CanDerivateLemmatization { get; set; }
    }
    
    public class EditionPrintPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.EditionPrint;

        public bool CanEditionPrintText { get; set; }
    }

    public class EditStaticTextPermissionContract : SpecialPermissionContract
    {
        public override SpecialPermissionTypeContract Key => SpecialPermissionTypeContract.EditStaticText;

        public bool CanEditStaticText { get; set; }
    }
}