using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.Dictionaries
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/searchjs").Include(
               "~/wwwroot/Areas/Dictionaries/js/itjakub.dictionaries.search.js",
               "~/wwwroot/Areas/Dictionaries/js/itjakub.dictionariesViewer.js"));

            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/headwordsjs").Include(
               "~/wwwroot/Areas/Dictionaries/js/itjakub.dictionaries.headwords.js",
               "~/wwwroot/Areas/Dictionaries/js/itjakub.dictionariesFavoriteHeadwords.js",
               "~/wwwroot/Areas/Dictionaries/js/itjakub.dictionariesViewer.js"));

            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/feedbackjs").Include(
               "~/wwwroot/Areas/Dictionaries/js/itjakub.dictionaries.feedback.js"));

            bundles.Add(new ScriptBundle("~/itjakub/dictionaries/listjs").Include(
               "~/wwwroot/Areas/Dictionaries/js/itjakub.dictionaries.list.js"));
        }
    }
}