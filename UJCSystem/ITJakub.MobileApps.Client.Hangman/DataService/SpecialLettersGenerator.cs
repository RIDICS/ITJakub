using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public class SpecialLettersGenerator
    {
        private const string SpecialLetters = "äāąćèľłḿǫṕŕſüūźъьěščřžťďňýáíéóúů";
        private const int DefaultRandomCount = 10;

        public ISet<char> GetSpecialLetters(IEnumerable<string> wordList)
        {
            var charSet = new HashSet<char>();
            foreach (var word in wordList)
            {
                foreach (var letter in word)
                {
                    if (letter == ' ' || (letter >= 'A' && letter <= 'Z') || (letter >= 'a' && letter <= 'z'))
                        continue;
                    charSet.Add(letter);
                }
            }
            return charSet;
        }

        public ISet<char> GetSpecialLettersWithRandom(IEnumerable<string> wordList, int randomCount = DefaultRandomCount)
        {
            var letters = GetSpecialLetters(wordList);
            AddRandomLetters(letters, randomCount);
            return letters;
        }

        private void AddRandomLetters(ISet<char> charSet, int count)
        {
            // Avoiding infinite loop
            if (charSet.Count + count >= SpecialLetters.Length)
            {
                count = (SpecialLetters.Length - charSet.Count) / 2;
            }

            var randomGenerator = new Random();
            for (int i = 0; i < count; i++)
            {
                var randomIndex = randomGenerator.Next(SpecialLetters.Length);
                var letter = SpecialLetters[randomIndex];
                while (charSet.Contains(letter))
                {
                    randomIndex = randomGenerator.Next(SpecialLetters.Length);
                    letter = SpecialLetters[randomIndex];
                }
                charSet.Add(letter);
            }
        }
    }
}