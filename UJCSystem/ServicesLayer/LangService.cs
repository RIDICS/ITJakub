using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServicesLayer
{
    public class LangService : ILangService
    {
        public string RemoveTags(string text)
        {
            bool tagMark = true;
            string output = "";
            foreach (char ch in text)
            {
                if (ch == '<')
                {
                    tagMark = true;
                    continue;
                }
                if (ch == '>')
                {
                    tagMark = false;
                    continue;
                }
                if (!tagMark)
                    output += ch;
            }

            return output;
        }


        public string GetLemma(string word)
        {
            return word;
        }

        public string GetStemma(string word)
        {
            return word;
        }

        public string[] GetSenetces(string text, string word)
        {
            List<string> ret = new List<string>();
            int startPos = 0;
            int foundPos = 0;
            while (foundPos >= 0)
            {
                foundPos = text.IndexOf(word, startPos);
                ret.Add(GetSenetce(text,word,foundPos));
                startPos = foundPos;
            }

            return ret.ToArray();
        }

        private string GetSenetce(string text, string word, int pos)
        {
            int startSentence = 0;
            for (int i = pos; i >= 0; --i)
                if (text[i] == '.')
                    startSentence = i + 1;

            int stopSentence = text.IndexOf('.', pos);
            if (stopSentence < 0)
                stopSentence = text.Length;

            int sentenceLength = stopSentence - startSentence;

            return text.Substring(startSentence, sentenceLength);
        }


        public bool WordOrder(string text, string[] words)
        {
            int pos = 0;
            foreach (string word in words)
            {
                int wordPos = text.IndexOf(word);
                if (wordPos < pos || wordPos < 0)
                    return false;
                pos = wordPos;
            }

            return true;
        }


        public string[] DivideToSubsentences(string text)
        {
            List<string> ret = new List<string>();
            int startPos = 0;
            int foundPos = 0;
            while (foundPos >= 0)
            {
                foundPos = text.IndexOf(",", startPos);
                if (foundPos >= 0)
                    ret.Add(text.Substring(startPos,foundPos-startPos));

                startPos = foundPos;
            }

            return ret.ToArray();
        }

        public bool ContainsWords(string text, string[] words)
        {
            foreach(string word in words)
                if (!text.Contains(word))
                    return false;

            return true;
        }
    }
}
