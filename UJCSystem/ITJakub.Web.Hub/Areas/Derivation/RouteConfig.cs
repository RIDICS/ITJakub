using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Derivation
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Derivation_default",
                "Derivation/{controller}/{action}/{id}",
                new { controller = "Derivation", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}