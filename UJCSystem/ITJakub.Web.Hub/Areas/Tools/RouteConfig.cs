using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Tools
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Tools_default",
                "Tools/{controller}/{action}/{id}",
                new { controller = "Tools", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}