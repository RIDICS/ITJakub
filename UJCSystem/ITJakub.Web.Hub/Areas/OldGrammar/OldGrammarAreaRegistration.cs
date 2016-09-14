using System.Web.Mvc;

namespace ITJakub.Web.Hub.Areas.OldGrammar
{
    public class OldGrammarAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "OldGrammar"; }
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