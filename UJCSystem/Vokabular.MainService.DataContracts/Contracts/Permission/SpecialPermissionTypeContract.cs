using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpecialPermissionTypeContract
    {
        News,
        UploadBook,
        Permissions,
        Feedback,
        CardFile,
        AutoImport,
        ReadLemmatization,
        EditLemmatization,
        DerivateLemmatization,
        EditionPrint,
        EditStaticText,
    }
}