using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Lemmatization
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/lemmatization/javascript").Include(
                "~/Areas/Lemmatization/Scripts/itjakub.lemmatization.js",
                "~/Areas/Lemmatization/Scripts/itjakub.lemmatization.list.js",
                "~/wwwroot/js/Plugins/Lemmatization/itjakub.lemmatization.shared.js"));

            bundles.Add(new StyleBundle("~/itjakub/lemmatization/css")
                .Include("~/wwwroot/Areas/Lemmatization/css/itjakub.lemmatization.css"));
        }
    }
}