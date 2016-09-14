using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.BohemianTextBank
{
    public class BohemianTextBankAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "BohemianTextBank"; }
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