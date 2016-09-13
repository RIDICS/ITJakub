using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Bibliographies
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/bibliographies/javascript").Include(
                "~/wwwroot/Areas/Bibliographies/js/itjakub.bibliographies.search.js"));
        }
    }
}