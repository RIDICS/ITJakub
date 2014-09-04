using System.Web.Mvc;
using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Editions
{
    public class EditionsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Editions"; }
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