using System.Globalization;
using System.Linq;
using System.Text;
using Localization.AspNetCore.Service;
using Localization.CoreLibrary.Manager;
using Localization.CoreLibrary.Util;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace ITJakub.Web.Hub.Helpers
{
    public static class BreadcrumbsHelper
    {
        public static HtmlString Create(HttpContext httpContext, params string[] path)
        {
            string cultureName = httpContext.Request.Cookies[ServiceBase.CultureCookieName];

            IAutoLocalizationManager localizer = Localization.CoreLibrary.Localization.Translator;
            if (string.IsNullOrEmpty(cultureName))
            {
                cultureName = localizer.DefaultCulture().ToString();
            }

            CultureInfo cultureInfo = new CultureInfo(cultureName);
            string appNameLocalized = localizer.Translate(LocTranslationSource.File, "VokabulářWebový", cultureInfo, "global");


            var sb = new StringBuilder();
            sb.Append("<ul class=\"breadcrumb\">");

            //sb.Append("<li><a href=\"/\">Vokabulář webový</a></li> ");
            sb.Append("<li><a href=\"/\">");
            sb.Append(appNameLocalized);
            sb.Append("</a></li> ");

            for (int i = 0; i < path.Length - 1; i++)
            {
                sb.AppendFormat("<li><a href=\"#\">{0}</a></li> ", path[i]);
            }
            sb.AppendFormat("<li class=\"active\">{0}</li> ", path.Last());

            sb.Append("</ul>");

            return new HtmlString(sb.ToString());
        }
    }
}