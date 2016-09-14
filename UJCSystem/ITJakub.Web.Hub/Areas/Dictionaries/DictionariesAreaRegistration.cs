using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Dictionaries
{
    public class DictionariesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Dictionaries"; }
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