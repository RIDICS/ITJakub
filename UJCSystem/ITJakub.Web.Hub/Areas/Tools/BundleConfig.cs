using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Tools
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/tools/javascript").Include(
            "~/Areas/Tools/Scripts/itjakub.tools.js"));

            bundles.Add(new StyleBundle("~/itjakub/tools/css").Include(
                "~/Areas/Tools/Content/itjakub.tools.css"));
        }
    }
}