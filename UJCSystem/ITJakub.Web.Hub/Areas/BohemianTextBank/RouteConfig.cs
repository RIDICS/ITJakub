using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.BohemianTextBank
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BohemianTextBank_default",
                "BohemianTextBank/{controller}/{action}/{id}",
                new { controller = "BohemianTextBank", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}