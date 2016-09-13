using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.BohemianTextBank
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/bohemiantextbank/javascript").Include(
                "~/wwwroot/Areas/BohemianTextBank/js/itjakub.bohemiatextbank.modul.inicializator.js",
                "~/wwwroot/Areas/BohemianTextBank/js/itjakub.bohemiantextbank.search.js",
                "~/wwwroot/Areas/BohemianTextBank/js/itjakub.bohemiantextbank.list.js"));
        }
    }
}