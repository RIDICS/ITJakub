using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.OldGrammar
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OldGrammar_default",
                "OldGrammar/{controller}/{action}/{id}",
                new {controller = "OldGrammar", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}