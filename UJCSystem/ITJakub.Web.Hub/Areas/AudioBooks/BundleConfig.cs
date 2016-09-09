using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.AudioBooks
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/audiobooks/javascript").Include(
                "~/Areas/AudioBooks/Scripts/itjakub.audiobooks.modul.inicializator.js",
                "~/Areas/AudioBooks/Scripts/itjakub.audiobooks.js"));

            bundles.Add(new StyleBundle("~/itjakub/audiobooks/css").Include(
                "~/wwwroot/Areas/AudioBooks/css/itjakub.audiobooks.css"));
        }
    }
}