using System.Web.Optimization;

namespace ITJakub.Web.Hub
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ----- Scripts -----

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/wwwroot/lib/jquery/dist/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/wwwroot/lib/jquery-ui/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/wwwroot/lib/jquery-validation/dist/jquery.validate.js",
                "~/wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryappear").Include(
                "~/wwwroot/lib/jquery-appear/src/jquery.appear.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/wwwroot/lib/modernizr/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/eucookies").Include(
                "~/wwwroot/lib/jquery-eu-cookie-law-popup/js/jquery-eu-cookie-law-popup.js",
                "~/wwwroot/js/Plugins/itjakub.eucookiepopup.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/wwwroot/lib/bootstrap/dist/js/bootstrap.js",
                "~/wwwroot/lib/respond/dest/respond.src.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                "~/wwwroot/lib/dropzone/downloads/dropzone.js"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
                "~/wwwroot/lib-custom/typeahead.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/keyboardLayout").Include(
                "~/wwwroot/js/Plugins/Keyboard/itjakub.plugins.keyboard.js",
                "~/wwwroot/js/Plugins/Keyboard/itjakub.plugins.keyboardComponent.js",
                "~/wwwroot/js/Plugins/Keyboard/itjakub.plugins.keyboardManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/storage").Include(
                "~/wwwroot/js/Plugins/Storage/itjakub.plugins.storage.manager.js",
                "~/wwwroot/js/Plugins/Storage/itjakub.plugins.storage.cookiestorage.js",
                "~/wwwroot/js/Plugins/Storage/itjakub.plugins.storage.localstorage.js"));

            bundles.Add(new ScriptBundle("~/bundles/imagezoom").Include(
                "~/wwwroot/lib/Wheelzoom/wheelzoom.js",
                "~/wwwroot/lib/jquery-zoom/jquery.zoom.js"));

            bundles.Add(new ScriptBundle("~/bundles/simplemde").Include(
                "~/wwwroot/lib/simplemde/dist/simplemde.min.js"));

            bundles.Add(new ScriptBundle("~/itjakub/javascript").Include(
                "~/wwwroot/js/itjakub.js",
                "~/wwwroot/js/Plugins/Progress/itjakub.plugins.progress.js",
                "~/wwwroot/js/Plugins/itjakub.modul.inicializator.js",
                "~/wwwroot/js/Plugins/itjakub.list.modul.inicializator.js",
                "~/wwwroot/js/Plugins/itjakub.search.modul.inicializator.js",
                "~/wwwroot/js/Plugins/Reader/itjakub.plugins.reader.js",
                "~/wwwroot/js/Plugins/Bibliography/itjakub.plugins.bibliography.variableInterpreter.js",
                "~/wwwroot/js/Plugins/Bibliography/itjakub.plugins.bibliography.factories.js",
                "~/wwwroot/js/Plugins/Bibliography/itjakub.plugins.bibliography.configuration.js",
                "~/wwwroot/js/Plugins/Bibliography/itjakub.plugins.bibliography.js",
                "~/wwwroot/js/Plugins/Sort/itjakub.plugins.sort.js",
                "~/wwwroot/js/Plugins/DropdownSelect/itjakub.plugins.dropdownselect.js",
                "~/wwwroot/js/Plugins/DropdownSelect/itjakub.plugins.dropdownselect2.js",
                "~/wwwroot/js/Plugins/RegExSearch/itjakub.plugins.regexsearch.js",
                "~/wwwroot/js/Plugins/itjakub.plugins.pagination.js",
                "~/wwwroot/js/Plugins/SearchBox/itjakub.plugins.searchbox.js",
                "~/wwwroot/js/Plugins/SearchBox/itjakub.plugins.singlesearchbox.js"));

            bundles.Add(new ScriptBundle("~/itjakub/home/javascript").Include(
                "~/wwwroot/js/itjakub.home.js"));

            bundles.Add(new ScriptBundle("~/itjakub/news/javascript").Include(
                "~/wwwroot/js/itjakub.news.js"));

            bundles.Add(new ScriptBundle("~/itjakub/feedback/javascript").Include(
                "~/wwwroot/js/itjakub.feedbacks.js"));

            bundles.Add(new ScriptBundle("~/itjakub/upload/javascript").Include(
                "~/wwwroot/js/itjakub.upload.js"));

            bundles.Add(new ScriptBundle("~/itjakub/permission/user/javascript").Include(
                "~/wwwroot/js/Permission/itjakub.permission.user.js"));

            bundles.Add(new ScriptBundle("~/itjakub/permission/group/javascript").Include(
                "~/wwwroot/js/Permission/itjakub.permission.group.js"));

            bundles.Add(new ScriptBundle("~/itjakub/text/javascript").Include(
                "~/wwwroot/js/itjakub.text.editor.js"));


            // ----- Styles ------

            bundles.Add(new StyleBundle("~/itjakub/permission/css").Include(
                "~/wwwroot/css/ITJakub.Permission.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/wwwroot/lib/bootstrap/dist/css/bootstrap.css",
                "~/wwwroot/css/site.css"));

            bundles.Add(new StyleBundle("~/Content/eucookiescss").Include(
                "~/wwwroot/lib/jquery-eu-cookie-law-popup/css/jquery-eu-cookie-law-popup.css",
                "~/wwwroot/css/ITJakub.EuCookiePopup.css"));

            bundles.Add(new StyleBundle("~/Content/itjakub").Include(
                "~/wwwroot/css/ITJakub.css"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                "~/wwwroot/lib/dropzone/downloads/css/basic.css",
                "~/wwwroot/lib/dropzone/downloads/css/dropzone.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryuicss").Include(
                "~/wwwroot/lib/jquery-ui/themes/base/*.css"));

            bundles.Add(new StyleBundle("~/Content/simplemdecss").Include(
                "~/wwwroot/lib/simplemde/dist/simplemde.min.css",
                "~/wwwroot/css/ITJakub.TextEditor.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}