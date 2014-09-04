using System.Collections.Generic;
using ITJakub.MobileApps.Client.Hangman.DataContract;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public class HangmanTask
    {
        private const int FullLiveCount = 11;

        private readonly string[] m_specifiedWords;
        private readonly HashSet<char> m_guessedLetterSet;
        private char[] m_guessedLetters;
        private int m_lives;
        private int m_currentWordIndex;

        public HangmanTask(string[] specifiedWords)
        {
            m_specifiedWords = specifiedWords;
            m_currentWordIndex = -1;

            Lives = FullLiveCount;
            m_guessedLetterSet = new HashSet<char>();

            PrepareNewWord();
        }

        public int Lives
        {
            get { return m_lives; }
            private set
            {
                m_lives = value;
                if (m_lives == 0)
                    m_guessedLetters = m_specifiedWords[m_currentWordIndex].ToCharArray();
            }
        }

        public string GuessedLetters
        {
            get { return new string(m_guessedLetters); }
        }

        public bool Win { get; private set; }

        public bool Loss { get { return m_lives == 0; } }

        public int WordOrder { get { return m_currentWordIndex; } }
        public bool IsNewWord { get; private set; }

        private void PrepareNewWord()
        {
            m_currentWordIndex++;
            if (m_currentWordIndex >= m_specifiedWords.Length)
            {
                Win = true;
                return;
            }

            // Convert current word to upper case (for comparing with guessed letters)
            m_specifiedWords[m_currentWordIndex] = m_specifiedWords[m_currentWordIndex].ToUpper();
            var currentWord = m_specifiedWords[m_currentWordIndex];

            m_guessedLetterSet.Clear();
            m_guessedLetters = new char[currentWord.Length];

            for (var i = 0; i < currentWord.Length; i++)
                m_guessedLetters[i] = currentWord[i] == ' ' ? ' ' : (char)0;

            IsNewWord = true;
        }

        public void Guess(char letter)
        {
            IsNewWord = false;
            if (m_guessedLetterSet.Contains(letter) || Loss || Win)
                return;

            m_guessedLetterSet.Add(letter);
            var currentWord = m_specifiedWords[m_currentWordIndex];
            var containsLetter = false;
            var containsNonGuessedLetter = false;

            for (var i = 0; i < currentWord.Length; i++)
            {
                if (letter == currentWord[i])
                {
                    containsLetter = true;
                    m_guessedLetters[i] = currentWord[i];
                }
                if (m_guessedLetters[i] == 0)
                {
                    containsNonGuessedLetter = true;
                }
            }

            if (!containsLetter)
                Lives--;

            if (!containsNonGuessedLetter)
                PrepareNewWord();
        }

        public void Guess(GuessLetter guessLetterObject)
        {
            // If user guessing letter in old word (word has been guessed)
            if (guessLetterObject.WordOrder > m_currentWordIndex)
                return;

            Guess(guessLetterObject.Letter);
        }
    }
}