using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.OldGrammar
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/oldgrammar/javascript").Include(
                "~/Areas/OldGrammar/Scripts/itjakub.oldgrammar.js"));

            bundles.Add(new StyleBundle("~/itjakub/oldgrammar/css").Include(
                "~/Areas/OldGrammar/Content/itjakub.oldgrammar.css"));
        }
    }
}