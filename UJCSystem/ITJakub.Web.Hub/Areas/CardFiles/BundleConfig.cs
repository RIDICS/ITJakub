using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.CardFiles
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/cardfiles/javascript").Include(
            "~/Areas/CardFiles/Scripts/itjakub.cardfiles.js",
            "~/Areas/CardFiles/Scripts/itjakub.cardfileCreator.js"));

            bundles.Add(new StyleBundle("~/itjakub/cardfiles/css").Include(
                "~/Areas/CardFiles/Content/itjakub.cardfiles.css",
                "~/Areas/CardFiles/Content/itjakub.cardfile.css"));
        }
    }
}