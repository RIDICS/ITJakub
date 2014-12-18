using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Bibliographies
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Bibliographies_default",
                "Bibliographies/{controller}/{action}/{id}",
                new { controller = "Bibliographies", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}