using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Lemmatization
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Lemmatization_default",
                "Lemmatization/{controller}/{action}/{id}",
                new {controller = "Lemmatization", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}