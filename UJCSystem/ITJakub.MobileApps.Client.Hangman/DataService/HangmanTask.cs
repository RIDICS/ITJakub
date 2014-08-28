using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public class HangmanTask
    {
        private const int FullLiveCount = 10;

        private readonly string m_word;
        private readonly HashSet<char> m_guessedLetters;
        private readonly char[] m_guessed;

        public HangmanTask(string word)
        {
            m_word = word;
            m_guessed = new char[word.Length];
            Lives = FullLiveCount;
            m_guessedLetters = new HashSet<char>();

            for (int i = 0; i < word.Length; i++)
                m_guessed[i] = word[i] == ' ' ? ' ' : (char)0;
        }

        public int Lives { get; private set; }

        public string GuessedLetters
        {
            get { return new string(m_guessed); }
        }

        public void Guess(char letter)
        {
            if (m_guessedLetters.Contains(letter))
                return;

            m_guessedLetters.Add(letter);
            var containsLetter = false;

            for (int i = 0; i < m_word.Length; i++)
            {
                if (letter == m_word[i])
                {
                    containsLetter = true;
                    m_guessed[i] = m_word[i];
                }
            }

            if (!containsLetter)
                Lives--;
        }
    }
}