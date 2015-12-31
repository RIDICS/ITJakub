using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Bibliographies
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/bibliographies/javascript").Include(
            "~/Areas/Bibliographies/Scripts/itjakub.bibliographies.search.js"));

            bundles.Add(new StyleBundle("~/itjakub/bibliographies/css").Include(
                "~/Areas/Bibliographies/Content/itjakub.bibliographies.css"));
        }
    }
}