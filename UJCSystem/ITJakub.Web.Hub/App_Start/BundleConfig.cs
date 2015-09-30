﻿using System.Web.Optimization;

namespace ITJakub.Web.Hub
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryappear").Include(
                "~/Scripts/jquery.appear.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/cookies").Include("~/Scripts/cookies/jquery-eu-cookie-law-popup.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     "~/Scripts/dropzone/dropzone.js"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
                "~/Scripts/typeahead.bundle.js"));

            bundles.Add(new ScriptBundle("~/itjakub/javascript").Include(
                "~/Scripts/itjakub.js",
                "~/Scripts/Plugins/Search/itjakub.plugins.search.js",
                "~/Scripts/Plugins/Reader/itjakub.plugins.reader.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.variableInterpreter.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.factories.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.sorting.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.configuration.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.js",
                "~/Scripts/Plugins/DropdownSelect/itjakub.plugins.dropdownselect.js",
                "~/Scripts/Plugins/DropdownSelect/itjakub.plugins.dropdownselect2.js",
                "~/Scripts/Plugins/RegExSearch/itjakub.plugins.regexsearch.js",
                "~/Scripts/Plugins/itjakub.plugins.pagination.js",
                "~/Scripts/Plugins/SearchBox/itjakub.plugins.searchbox.js"));

            bundles.Add(new ScriptBundle("~/itjakub/home/javascript").Include(
                "~/Scripts/Home/itjakub.home.js"));
            bundles.Add(new ScriptBundle("~/itjakub/news/javascript").Include("~/Scripts/News/itjakub.news.js"));

            bundles.Add(new ScriptBundle("~/itjakub/lemmatization/javascript").Include(
                "~/Scripts/Lemmatization/itjakub.lemmatization.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/cookiescss").Include("~/Scripts/cookies/css/jquery-eu-cookie-law-popup.css"));

            bundles.Add(new StyleBundle("~/Content/itjakub").Include("~/Content/ITJakub.css"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                     "~/Scripts/dropzone/css/basic.css",
                     "~/Scripts/dropzone/css/dropzone.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryuicss").Include(
              "~/Content/themes/base/*.css"));

            bundles.Add(new StyleBundle("~/itjakub/lemmatization/css")
                .Include("~/Content/Lemmatization/itjakub.lemmatization.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}