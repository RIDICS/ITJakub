using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Editions
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/editions/javascript").Include(
            "~/Areas/Editions/Scripts/itjakub.editions.js"));

            bundles.Add(new StyleBundle("~/itjakub/editions/css").Include(
                "~/Areas/Editions/Content/itjakub.editions.css"));
        }
    }
}