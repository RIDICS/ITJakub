using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Editions
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Editions_default",
                "Editions/{controller}/{action}/{id}",
                new {controller = "Editions", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}