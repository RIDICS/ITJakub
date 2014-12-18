using System.Web.Mvc;
using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Bibliographies
{
    public class BibliographiesAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get { return "Bibliographies"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RegisterRoutes(context);
            RegisterBundles();
        }

        private void RegisterRoutes(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context);
        }

        private void RegisterBundles()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}