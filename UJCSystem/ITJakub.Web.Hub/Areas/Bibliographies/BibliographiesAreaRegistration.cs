using System.Web.Mvc;

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
        }

        private void RegisterRoutes(AreaRegistrationContext context)
        {
            RouteConfig.RegisterRoutes(context);
        }
    }
}