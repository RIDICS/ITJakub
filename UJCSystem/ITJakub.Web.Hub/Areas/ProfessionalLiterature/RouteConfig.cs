using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.ProfessionalLiterature
{
    internal static class RouteConfig
    {
        internal static void RegisterRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProfessionalLiterature_default",
                "ProfessionalLiterature/{controller}/{action}/{id}",
                new {controller = "ProfessionalLiterature", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}