using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.CardFiles
{
    public class CardFilesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "CardFiles"; }
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