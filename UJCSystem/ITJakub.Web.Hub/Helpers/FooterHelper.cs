using System;
using Scalesoft.Localization.AspNetCore;
using Scalesoft.Localization.Core.Common;

namespace ITJakub.Web.Hub.Helpers
{
    public static class FooterHelper
    {
        public static string VokabularStartedLocalized(ILocalizationService localizer, DateTime releaseDate)
        {
            Guard.ArgumentNotNull(nameof(releaseDate), releaseDate);    

            var now = DateTime.Now;
            var difference = now - releaseDate;
            var age = DateTime.MinValue + difference; //Min value is 1.1. 0001

            int years = age.Year - 1;
            int months = age.Month - 1;
            int days = age.Day - 1;

            string yearsLabel = localizer.TranslatePluralization("Years", years, "global");
            string monthsLabel = localizer.TranslatePluralization("Months", years, "global");
            string daysLabel = localizer.TranslatePluralization("Days", years, "global");
            string andConjunction = localizer.Translate("and", "global");

            var resultString = $"{years} {yearsLabel}, {months} {monthsLabel} {andConjunction} {days} {daysLabel}";
            return resultString;
        }
    }
}
