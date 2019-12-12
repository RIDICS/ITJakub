using ITJakub.Web.Hub.Areas.Admin.Controllers.Constants;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace ITJakub.Web.Hub.Helpers
{
    public static class ViewExtensions
    {
        public static bool TryGetBooleanValue(this ViewDataDictionary viewData, string key)
        {
            return viewData.TryGetValue(key, out var value) && (bool) value;
        }

        public static PermissionDataContract HasPermissionFor(this ViewDataDictionary viewData)
        {
            if (viewData.TryGetValue(ProjectConstants.CurrentUserPermissions, out var value))
            {
                return value as PermissionDataContract;
            }
            return new PermissionDataContract();
        }
    }
}