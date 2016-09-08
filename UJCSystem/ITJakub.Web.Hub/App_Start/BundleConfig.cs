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

            bundles.Add(new ScriptBundle("~/bundles/cookies").Include(
                "~/wwwroot/lib/jquery-eu-cookie-law-popup/js/jquery-eu-cookie-law-popup.js",
                "~/Scripts/Plugins/itjakub.eucookiepopup.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/wwwroot/lib/bootstrap/dist/js/bootstrap.js",
                "~/wwwroot/lib/respond/dest/respond.src.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                "~/wwwroot/lib/dropzone/downloads/dropzone.js"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
                "~/Scripts/typeahead.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/keyboardLayout").Include(
                "~/Scripts/Plugins/Keyboard/itjakub.plugins.keyboard.js",
                "~/Scripts/Plugins/Keyboard/itjakub.plugins.keyboardComponent.js",
                "~/Scripts/Plugins/Keyboard/itjakub.plugins.keyboardManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/storage").Include(
                "~/Scripts/Plugins/Storage/itjakub.plugins.storage.manager.js",
                "~/Scripts/Plugins/Storage/itjakub.plugins.storage.cookiestorage.js",
                "~/Scripts/Plugins/Storage/itjakub.plugins.storage.localstorage.js"));

            bundles.Add(new ScriptBundle("~/bundles/imagezoom").Include(
                "~/wwwroot/lib/Wheelzoom/wheelzoom.js",
                "~/wwwroot/lib/jquery-zoom/jquery.zoom.js"));

            bundles.Add(new ScriptBundle("~/bundles/simplemde").Include(
                "~/wwwroot/lib/simplemde/dist/simplemde.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/colorpicker").Include(
                "~/wwwroot/lib/bootstrap-colorpicker-plus/dist/js/bootstrap-colorpicker.js",
                "~/wwwroot/lib/bootstrap-colorpicker-plus/dist/js/bootstrap-colorpicker-plus.js"));

            bundles.Add(new ScriptBundle("~/itjakub/javascript").Include(
                "~/Scripts/itjakub.js",
                "~/Scripts/Plugins/Progress/itjakub.plugins.progress.js",
                "~/Scripts/Plugins/itjakub.modul.inicializator.js",
                "~/Scripts/Plugins/itjakub.list.modul.inicializator.js",
                "~/Scripts/Plugins/itjakub.search.modul.inicializator.js",
                "~/Scripts/Plugins/Search/itjakub.plugins.search.js",
                "~/Scripts/Plugins/Reader/itjakub.plugins.reader.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.variableInterpreter.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.factories.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.configuration.js",
                "~/Scripts/Plugins/Bibliography/itjakub.plugins.bibliography.js",
                "~/Scripts/Plugins/Sort/itjakub.plugins.sort.js",
                "~/Scripts/Plugins/DropdownSelect/itjakub.plugins.dropdownselect.js",
                "~/Scripts/Plugins/DropdownSelect/itjakub.plugins.dropdownselect2.js",
                "~/Scripts/Plugins/RegExSearch/itjakub.plugins.regexsearch.js",
                "~/Scripts/Plugins/itjakub.plugins.pagination.js",
                "~/Scripts/Plugins/itjakub.tools.js",
                "~/Scripts/Plugins/SearchBox/itjakub.plugins.searchbox.js"));

            bundles.Add(new ScriptBundle("~/itjakub/home/javascript").Include(
                "~/Scripts/Home/itjakub.home.js"));

            bundles.Add(new ScriptBundle("~/itjakub/news/javascript").Include(
                "~/Scripts/News/itjakub.news.js"));

            bundles.Add(new ScriptBundle("~/itjakub/permission/user/javascript").Include(
                "~/Scripts/Permission/itjakub.permission.user.js",
                "~/Scripts/Plugins/SearchBox/itjakub.plugins.singlesearchbox.js"));

            bundles.Add(new ScriptBundle("~/itjakub/permission/group/javascript").Include(
                "~/Scripts/Permission/itjakub.permission.group.js",
                "~/Scripts/Plugins/SearchBox/itjakub.plugins.singlesearchbox.js"));

            bundles.Add(new ScriptBundle("~/itjakub/text/javascript").Include(
                "~/Scripts/Text/itjakub.text.editor.js"));

            bundles.Add(new ScriptBundle("~/itjakub/favorite/javascript").Include(
                "~/Scripts/Favorite/itjakub.favoriteManager.js",
                "~/Scripts/Favorite/itjakub.favoriteStar.js",
                "~/Scripts/Favorite/itjakub.favoriteQuery.js",
                "~/Scripts/Favorite/itjakub.newFavoriteDialog.js"));

            bundles.Add(new ScriptBundle("~/itjakub/favorite/management/javascript").Include(
                "~/Scripts/Favorite/itjakub.favoriteManagement.js"));


            // ----- Styles ------

            bundles.Add(new StyleBundle("~/itjakub/permission/css").Include(
                "~/Content/Permission/itjakub.permission.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/wwwroot/lib/bootstrap/dist/css/bootstrap.css",
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/cookiescss").Include(
                "~/wwwroot/lib/jquery-eu-cookie-law-popup/css/jquery-eu-cookie-law-popup.css",
                "~/Content/ITJakub.EuCookiePopup.css"));

            bundles.Add(new StyleBundle("~/Content/itjakub").Include(
                "~/Content/ITJakub.css",
                "~/Content/ITJakub.Favorites.css"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                "~/wwwroot/lib/dropzone/downloads/css/basic.css",
                "~/wwwroot/lib/dropzone/downloads/css/dropzone.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryuicss").Include(
                "~/wwwroot/lib/jquery-ui/themes/base/*.css"));

            bundles.Add(new StyleBundle("~/Content/simplemdecss").Include(
                "~/wwwroot/lib/simplemde/dist/simplemde.min.css",
                "~/Content/ITJakub.TextEditor.css"));

            bundles.Add(new StyleBundle("~/Content/colorpicker").Include(
                "~/wwwroot/lib/bootstrap-colorpicker-plus/dist/css/bootstrap-colorpicker.min.css",
                "~/wwwroot/lib/bootstrap-colorpicker-plus/dist/css/bootstrap-colorpicker-plus.min.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}