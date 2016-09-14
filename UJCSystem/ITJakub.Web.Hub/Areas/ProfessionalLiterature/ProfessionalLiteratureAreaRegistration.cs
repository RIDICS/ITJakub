using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.ProfessionalLiterature
{
    public class ProfessionalLiteratureAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "ProfessionalLiterature"; }
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