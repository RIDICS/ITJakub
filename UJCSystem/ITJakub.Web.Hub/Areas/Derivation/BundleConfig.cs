using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Derivation
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/derivation/javascript").Include(
                "~/wwwroot/Areas/Derivation/js/itjakub.derivation.js",
                "~/wwwroot/js/Plugins/Lemmatization/itjakub.lemmatization.shared.js"));

            bundles.Add(new StyleBundle("~/itjakub/derivation/css")
                .Include("~/wwwroot/Areas/Derivation/css/itjakub.derivation.css"));
        }
    }
}