using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Dictionaries
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/javascript").Include(
               "~/Areas/Dictionaries/Scripts/itjakub.dictionaries.js"));

            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/searchjs").Include(
               "~/Areas/Dictionaries/Scripts/itjakub.dictionaries.search.js",
               "~/Areas/Dictionaries/Scripts/itjakub.dictionariesViewer.js"));

            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/headwordsjs").Include(
               "~/Areas/Dictionaries/Scripts/itjakub.dictionaries.headwords.js",
               "~/Areas/Dictionaries/Scripts/itjakub.dictionariesViewer.js"));

            bundles.Add(new StyleBundle("~/itjakub/dictionaries/css").Include(
                "~/Areas/Dictionaries/Content/itjakub.dictionaries.css"));
        }
    }
}