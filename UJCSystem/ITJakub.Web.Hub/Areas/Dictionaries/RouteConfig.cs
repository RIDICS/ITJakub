using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Dictionaries
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Dictionaries_default",
                "Dictionaries/{controller}/{action}/{id}",
                new {controller = "Dictionaries", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}