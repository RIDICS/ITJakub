using System.Web.Mvc;

namespace Ujc.Naki.ITJakub.WebGui.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}