using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.BohemianTextBank
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/bohemiantextbank/javascript").Include(
                "~/Areas/BohemianTextBank/Scripts/itjakub.bohemiatextbank.modul.inicializator.js",
                "~/Areas/BohemianTextBank/Scripts/itjakub.bohemiantextbank.search.js",
                "~/Areas/BohemianTextBank/Scripts/itjakub.bohemiantextbank.list.js"));

            bundles.Add(new StyleBundle("~/itjakub/bohemiantextbank/css").Include(
                "~/Areas/BohemianTextBank/Content/itjakub.bohemiantextbank.css"));
        }
    }
}