﻿using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace ITJakub.Web.Hub.Helpers
{
    public static class BreadcrumbsHelper
    {
        public static HtmlString Create(params string[] path)
        {
            var sb = new StringBuilder();
            sb.Append("<ul class=\"breadcrumb\">");

            sb.Append("<li><a href=\"/\">Vokabulář webový</a></li> ");
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