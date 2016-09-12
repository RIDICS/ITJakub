using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.OldGrammar
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/oldgrammar/javascript").Include(
                "~/wwwroot/Areas/OldGrammar/js/itjakub.oldgrammar.js"));

            bundles.Add(new ScriptBundle("~/itjakub/oldgrammar/listjs").Include(
                "~/wwwroot/Areas/OldGrammar/js/itjakub.oldgrammar.list.js"));

            bundles.Add(new ScriptBundle("~/itjakub/oldgrammar/searchjs").Include(
                "~/wwwroot/Areas/OldGrammar/js/itjakub.oldgrammar.search.js"));

            bundles.Add(new StyleBundle("~/itjakub/oldgrammar/css").Include(
                "~/wwwroot/Areas/OldGrammar/css/itjakub.oldgrammar.css"));
        }
    }
}