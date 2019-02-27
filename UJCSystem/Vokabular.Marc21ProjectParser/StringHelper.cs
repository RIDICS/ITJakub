﻿namespace Vokabular.Marc21ProjectParser
{
    internal static class StringHelper
    {
        //TODO doc
        public static string RemoveUnnecessaryCharacters(this string text)
        {
            var isLastUnnecessary = !char.IsLetterOrDigit(text[text.Length - 1]);

            if (isLastUnnecessary)
            {
                for (int i = text.Length - 1; i > 0; i--)
                {
                    if (char.IsLetterOrDigit(text[i]))
                    {
                        text = text.Substring(0, i + 1);
                        break;
                    }
                }
            }

            var isFirstUnnecessary = !char.IsLetterOrDigit(text[0]);

            if (isFirstUnnecessary)
            {
                for (int i = 0; i < text.Length-1; i++)
                {
                    if (char.IsLetterOrDigit(text[i]))
                    {
                        text = text.Substring(i);
                        break;
                    }
                }
            }

            return text;
        }
    }
}
