using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.AudioBooks
{
    public class AudioBooksAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "AudioBooks"; }
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