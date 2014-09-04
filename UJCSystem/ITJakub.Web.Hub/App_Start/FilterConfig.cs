using System.Web;
using System.Web.Mvc;

namespace ITJakub.Web.Hub
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
