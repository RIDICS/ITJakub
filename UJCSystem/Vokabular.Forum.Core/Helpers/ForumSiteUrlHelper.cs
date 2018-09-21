using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Vokabular.ForumSite.Core.Helpers
{
    public static class ForumSiteUrlHelper
    {
        public const string ForumSiteBaseUrl = "http://localhost:50165"; //TODO load from config file???
        private const string TopicsUrlPart = "/topics";

        public static string GetTopicsUrl(int forumId, string forumName)
        {
            return ForumSiteBaseUrl + TopicsUrlPart + "/" + forumId + "-" + CleanStringForURL(forumName);
        }

        public static string CleanStringForURL(string inputString)
        {
            var sb = new StringBuilder();

            inputString = inputString.Trim();
            inputString = inputString.Replace("&", "and").Replace("ـ", string.Empty);
            inputString = Regex.Replace(inputString, @"\p{Cs}", string.Empty);
            inputString = inputString.Normalize(NormalizationForm.FormD);


            foreach (char currentChar in inputString)
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