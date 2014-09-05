using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Dictionaries
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/javascript").Include(
               "~/Areas/Dictionaries/Scripts/itjakub.dictionaries.js"));

            bundles.Add(new StyleBundle("~/itjakub/dictionaries/css").Include(
                "~/Areas/Dictionaries/Content/itjakub.dictionaries.css"));
        }
    }
}