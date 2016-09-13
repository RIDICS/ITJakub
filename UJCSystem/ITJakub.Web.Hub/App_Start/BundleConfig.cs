using System.Web.Optimization;

namespace ITJakub.Web.Hub
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ----- Scripts -----
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/wwwroot/lib/jquery-validation/dist/jquery.validate.js",
                "~/wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/simplemde").Include(
                "~/wwwroot/lib/simplemde/dist/simplemde.min.js"));

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


            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}