using System;
using System.Globalization;
using Localization.AspNetCore.Service;
using Localization.CoreLibrary.Common;
using Localization.CoreLibrary.Manager;
using Localization.CoreLibrary.Util;
using Microsoft.AspNetCore.Html;

namespace ITJakub.Web.Hub.Helpers
{
    public static class FooterHelper
    {
        public static HtmlString VokabularStarted(DateTime releaseDate)
        {
            var now = DateTime.Now;
            var difference = now - releaseDate;
            var age = DateTime.MinValue + difference; //Min value is 1.1. 0001

            int years = age.Year - 1;
            int months = age.Month - 1;
            int days = age.Day - 1;

            string yearsLabel = "lety";
            string monthsLabel = "měsíci";
            string daysLabel = "dny";

            if (years == 1)
            {
                yearsLabel = "rokem";
            }
            if (months == 1)
            {
                monthsLabel = "měsícem";
            }
            if (days == 1)
            {
                daysLabel = "dnem";
            }

            var resultString = $"{years} {yearsLabel}, {months} {monthsLabel} a {days} {daysLabel}";
            return new HtmlString(resultString);
        }

        public static HtmlString VokabularStartedLocalized(DateTime releaseDate, string cultureName)
        {
            Guard.ArgumentNotNull(nameof(releaseDate), releaseDate);    

            var now = DateTime.Now;
            var difference = now - releaseDate;
            var age = DateTime.MinValue + difference; //Min value is 1.1. 0001

            int years = age.Year - 1;
            int months = age.Month - 1;
            int days = age.Day - 1;

            IAutoLocalizationManager localizer = Localization.CoreLibrary.Localization.Translator;
            if (string.IsNullOrEmpty(cultureName))
            {
                cultureName = localizer.DefaultCulture().ToString();
            }

            CultureInfo cultureInfo = new CultureInfo(cultureName);

            string yearsLabel = localizer.TranslatePluralization(LocTranslationSource.File, "Years", years, cultureInfo, "global");
            string monthsLabel = localizer.TranslatePluralization(LocTranslationSource.File, "Months", years, cultureInfo, "global");
            string daysLabel = localizer.TranslatePluralization(LocTranslationSource.File, "Days", years, cultureInfo, "global");
            string andConjunction = localizer.Translate(LocTranslationSource.File, "and", cultureInfo,"global");

            var resultString = $"{years} {yearsLabel}, {months} {monthsLabel} {andConjunction} {days} {daysLabel}";
            return new HtmlString(resultString);
        }
    }
}
