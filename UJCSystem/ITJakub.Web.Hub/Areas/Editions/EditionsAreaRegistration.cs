using System.Web.Mvc;

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
        }

        private void RegisterRoutes(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context);
        }
    }
}