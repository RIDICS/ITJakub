using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ujc.Naki.MVCWebLayer.Enums
{
    public static class SourcesViewModeExtensions
    {
        public static string ToCzechString(this SourcesViewMode mode)
        {
            switch (mode)
            {
                case SourcesViewMode.Autor:
                    return "Autor";
                case SourcesViewMode.Jmeno:
                    return "Název";
                default:
                    return string.Empty;
            }
        }
    }
}