using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.Lemmatization
{
    public class LemmatizationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get { return "Lemmatization"; }
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