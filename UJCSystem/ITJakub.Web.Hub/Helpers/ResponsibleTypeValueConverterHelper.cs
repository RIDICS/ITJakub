using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Helpers
{
    public static class ResponsibleTypeValueConverterHelper
    {
        public static string ConvertToString(ResponsibleTypeEnumContract responsibleType)
        {
            switch (responsibleType)
            {
                case ResponsibleTypeEnumContract.Editor:
                    return "Editor";
                case ResponsibleTypeEnumContract.Kolace:
                    return "Kolace";
                default:
                    return "Neznámý";
            }
        }
    }
}
