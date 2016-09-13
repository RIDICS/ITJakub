using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.CardFiles
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/cardfiles/javascript").Include(
                "~/wwwroot/Areas/CardFiles/js/itjakub.cardfiles.js",
                "~/wwwroot/Areas/CardFiles/js/itjakub.cardfileManager.js"));
        }
    }
}