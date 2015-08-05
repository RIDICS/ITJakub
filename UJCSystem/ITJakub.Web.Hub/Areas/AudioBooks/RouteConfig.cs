using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.AudioBooks
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AudioBooks_default",
                "AudioBooks/{controller}/{action}/{id}",
                new { controller = "AudioBooks", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}