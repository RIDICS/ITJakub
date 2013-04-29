using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ujc.Naki.MVCWebLayer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "SearchDetail",
                "Search/Detail/{searchTerm}",
                new { controller = "Search", action = "Detail", searchTerm = "" }
            );

            routes.MapRoute(
                "SourcesMain",
                "Sources",
                new { controller = "Sources", action = "Listing", mode = "Jmeno", alphabet = "A" }
            );

            routes.MapRoute(
                "SourcesListing",
                "Sources/Listing/{mode}/{alphabet}",
                new { controller = "Sources", action = "Listing", mode = "Jmeno", alphabet = "A" }
            );

            routes.MapRoute(
                "SourcesDetail",
                "Sources/Detail/{id}/{part}",
                new { controller = "Sources", action = "Detail", part = "Info"}
            );

            routes.MapRoute(
                "SourcesDetailSearch",
                "Sources/Hledat/{id}",
                new { controller = "Sources", action = "DetailHledat" }
            );

            routes.MapRoute(
                "SourcesGoThrough",
                "Sources/Prochazet/{id}/{page}",
                new { controller = "Sources", action = "Prochazet", page = 1 }
            );

            routes.MapRoute(
                "SourcesGoThroughWithId",
                "Sources/Prochazet/{id}",
                new { controller = "Sources", action = "Prochazet", id = 1 }
            );

            routes.MapRoute(
                "ModuleDetail",
                "Module/{id}",
                new { controller = "Modules", action = "Index" }
            );

            routes.MapRoute(
                "HomePage",
                "",
                new { controller = "Index", action = "Index" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new { controller = "Index", action = "Index" }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}