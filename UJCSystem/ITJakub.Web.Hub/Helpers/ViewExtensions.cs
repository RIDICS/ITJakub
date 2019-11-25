using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ITJakub.Web.Hub.Helpers
{
    public static class ViewExtensions
    {
        public static bool TryGetBooleanValue(this ViewDataDictionary viewData, string key)
        {
            return viewData.TryGetValue(key, out var value) && (bool) value;
        }
    }
}