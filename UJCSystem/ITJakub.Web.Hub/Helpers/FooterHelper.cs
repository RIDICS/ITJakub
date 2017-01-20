using System;
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
    }
}
