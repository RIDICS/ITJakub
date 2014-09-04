using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ITJakub.Web.Hub.Startup))]
namespace ITJakub.Web.Hub
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
