﻿using System.Web.Optimization;

namespace ITJakub.Web.Hub.Areas.AudioBooks
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/itjakub/audiobooks/javascript").Include(
                "~/wwwroot/Areas/AudioBooks/js/itjakub.audiobooks.modul.inicializator.js",
                "~/wwwroot/Areas/AudioBooks/js/itjakub.audiobooks.js"));
        }
    }
}