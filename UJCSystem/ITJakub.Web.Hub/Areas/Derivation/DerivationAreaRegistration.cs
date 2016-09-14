using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Derivation
{
    public class DerivationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get { return "Derivation"; }
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