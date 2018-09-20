using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Vokabular.ForumSite.Core.Helpers
{
    class ForumSiteUrlHelper
    {
        public const string ForumSiteBaseUrl = "http://localhost:50165"; //TODO load from config file???

        private const string topicsUrlPart = "/topics";

        public static string GetTopicsUrl(int forumId, string forumName)
        {
            return ForumSiteBaseUrl + topicsUrlPart + "/" + forumId + "-" + CleanStringForURL(forumName);
        }

        /// <summary>
        /// Cleans the string for URL.
        /// </summary>
        /// <param name="inputString">The input String.</param>
        /// <returns>
        /// The clean string for url.
        /// </returns>
        public static string CleanStringForURL(string inputString)
        {
            var sb = new StringBuilder();

            // trim...
            inputString = inputString.Trim();

            // fix ampersand...
            inputString = inputString.Replace("&", "and").Replace("ـ", string.Empty);

            inputString = Regex.Replace(inputString, @"\p{Cs}", string.Empty);

            // normalize the Unicode
            inputString = inputString.Normalize(NormalizationForm.FormD);


            /* string strUnidecode;

             try
             {
                 strUnidecode = inputString.Unidecode().Replace(" ", "-");
             }
             catch (Exception)
             {
                 strUnidecode = inputString;
             }*/
            string strUnidecode = inputString;

            foreach (char currentChar in strUnidecode)
            {
                if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                {
                    sb.Append('-');
                }
                else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                         && !char.IsSymbol(currentChar))
                {
                    sb.Append(currentChar);
                }
            }

            string strNew = sb.ToString();

            while (strNew.EndsWith("-"))
            {
                strNew = strNew.Remove(strNew.Length - 1, 1);
            }

            return strNew.Length.Equals(0) ? "Default" : strNew;
        }
    }
}