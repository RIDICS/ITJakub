using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.CardFiles
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/cardfiles/javascript").Include(
            "~/Areas/CardFiles/Scripts/itjakub.cardfiles.js",
            "~/Areas/CardFiles/Scripts/itjakub.cardfileManager.js"));

            bundles.Add(new StyleBundle("~/itjakub/cardfiles/css").Include(
                "~/wwwroot/Areas/CardFiles/css/itjakub.cardfiles.css",
                "~/wwwroot/Areas/CardFiles/css/itjakub.cardfile.css"));
        }
    }
}