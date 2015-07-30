using System;

namespace Ujc.Ovj.Xml.Tei.Contents
{
    public static class HeadwordCleaner
    {
        /// <summary>
        /// Cleans input text for sorting purposes.
        /// Removes non-letter characters from the beginning.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanForSorting(string text)
        {
            string result = null;
            if (text == null) return null;
            if (result == null)
            {
                int start = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (Char.IsLower(text[i]) || Char.IsUpper(text[i]))
                    {
                        if (i > 0) start = i;
                        break;
                    }
                }
                if (start == 0)
                {
                    result = text;
                }
                else
                {
                    result = text.Substring(start);
                }
            }
            return result;
        }
    }
}