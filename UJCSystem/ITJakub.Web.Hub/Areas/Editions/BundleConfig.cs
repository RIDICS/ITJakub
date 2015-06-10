using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Editions
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/editions/javascript").Include(
            "~/Areas/Editions/Scripts/itjakub.editions.js"));

            bundles.Add(new ScriptBundle("~/itjakub/editions/listjs").Include(
                "~/Areas/Editions/Scripts/itjakub.editions.list.js"));

            bundles.Add(new ScriptBundle("~/itjakub/editions/searchjs").Include(
                "~/Areas/Editions/Scripts/itjakub.editions.search.js"));

            bundles.Add(new StyleBundle("~/itjakub/editions/css").Include(
                "~/Areas/Editions/Content/itjakub.editions.css"));
        }
    }
}