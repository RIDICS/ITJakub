using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.ProfessionalLiterature
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/itjakub/professionalliterature/javascript").Include(
                "~/Areas/ProfessionalLiterature/Scripts/itjakub.professionalliterature.js"));

            bundles.Add(new ScriptBundle("~/itjakub/professionalliterature/listjs").Include(
                "~/Areas/ProfessionalLiterature/Scripts/itjakub.professionalliterature.modul.inicializator.js",
                "~/Areas/ProfessionalLiterature/Scripts/itjakub.professionalliterature.list.js"));

            bundles.Add(new ScriptBundle("~/itjakub/professionalliterature/searchjs").Include(
                "~/Areas/ProfessionalLiterature/Scripts/itjakub.professionalliterature.search.js"));

            bundles.Add(new StyleBundle("~/itjakub/professionalliterature/css").Include(
                "~/Areas/ProfessionalLiterature/Content/itjakub.professionalliterature.css"));
        }
    }
}