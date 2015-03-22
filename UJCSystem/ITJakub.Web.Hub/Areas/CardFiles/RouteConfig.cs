using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.CardFiles
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CardFiles_default",
                "CardFiles/{controller}/{action}/{id}",
                new { controller = "CardFiles", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}