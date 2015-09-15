using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.MobileApps.Client.Hangman.DataContract;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public class HangmanTask
    {
        private const int FullLiveCount = 15;
        private const int TotalHangmanPictureCount = 7;

        private readonly Random m_randomGenerator;
        private readonly string[] m_specifiedWords;
        private readonly string[] m_specifiedHints;
        private readonly HashSet<char> m_guessedLetterSet;
        private HashSet<int> m_usedHangmanPictures;
        private char[] m_guessedLetters;
        private int m_livesRemain;
        private int m_currentLevel;
        private bool m_isGuessFromHistory;

        public HangmanTask(HangmanTaskContract.WordContract[] specifiedWords)
        {
            //TODO generate new random default hangman
            m_randomGenerator = new Random();
            m_specifiedWords = specifiedWords.Select(x => x.Answer).ToArray();
            m_specifiedHints = specifiedWords.Select(x => x.Hint).ToArray();
            m_currentLevel = -1;

            LivesRemain = FullLiveCount;
            GuessedLetterCount = 0;
            m_guessedLetterSet = new HashSet<char>();

            PrepareNewWord();
        }

        public int LivesRemain
        {
            get { return m_livesRemain; }
            private set
            {
                m_livesRemain = value;
                if (m_livesRemain == 0)
                {
                    HangmanCount++;
                    m_livesRemain = FullLiveCount;
                    GenerateNewRandomHangmanPicture();
                }
            }
        }

        public int HangmanCount { get; private set; }

        public int HangmanPicture { get; private set; }

        public string GuessedLetters
        {
            get { return new string(m_guessedLetters); }
        }

        public string CurrentHint
        {
            get { return m_currentLevel < m_specifiedHints.Length ? m_specifiedHints[m_currentLevel] : string.Empty; }
        }

        public bool Win { get; private set; }

        public bool Loss { get { return m_livesRemain == 0; } }

        public int WordOrder { get { return m_currentLevel; } }

        public int GuessedLetterCount { get; set; }

        public bool IsNewWord { get; private set; }

        private void PrepareNewWord()
        {
            m_currentLevel++;
            if (m_currentLevel >= m_specifiedWords.Length)
            {
                Win = true;
                return;
            }

            // Convert current word to upper case (for comparing with guessed letters)
            m_specifiedWords[m_currentLevel] = m_specifiedWords[m_currentLevel].ToUpper();
            var currentWord = m_specifiedWords[m_currentLevel];

            m_guessedLetterSet.Clear();
            m_guessedLetters = new char[currentWord.Length];

            for (var i = 0; i < currentWord.Length; i++)
                m_guessedLetters[i] = currentWord[i] == ' ' ? ' ' : (char)0;

            IsNewWord = true;
        }

        public void Guess(char letter)
        {
            IsNewWord = false;
            letter = char.ToUpper(letter);
            if (m_guessedLetterSet.Contains(letter) || Loss || Win)
                return;

            m_guessedLetterSet.Add(letter);
            var currentWord = m_specifiedWords[m_currentLevel];
            var containsLetter = false;
            var containsNonGuessedLetter = false;

            for (var i = 0; i < currentWord.Length; i++)
            {
                if (letter == currentWord[i])
                {
                    containsLetter = true;
                    GuessedLetterCount++;
                    m_guessedLetters[i] = currentWord[i];
                }
                if (m_guessedLetters[i] == 0)
                {
                    containsNonGuessedLetter = true;
                }
            }

            if (!containsLetter)
                LivesRemain--;

            if (!containsNonGuessedLetter)
                PrepareNewWord();
        }

        public void Guess(ProgressInfoContract progressInfoObject)
        {
            // If user guessing letter in old word (word has been guessed)
            if (progressInfoObject.WordOrder < m_currentLevel)
                return;

            m_isGuessFromHistory = true;
            Guess(progressInfoObject.Letter);
            SetHangmanPicture(progressInfoObject.HangmanPicture);
            m_isGuessFromHistory = false;
        }

        private void SetHangmanPicture(int picture)
        {
            if (m_usedHangmanPictures == null)
            {
                m_usedHangmanPictures = new HashSet<int> {picture};
                HangmanPicture = picture;
                return;
            }

            if (HangmanPicture == picture)
                return;

            if (m_usedHangmanPictures.Count == TotalHangmanPictureCount)
            {
                m_usedHangmanPictures.Clear();
                m_usedHangmanPictures.Add(picture);
                HangmanPicture = picture;
            }
        }

        private void GenerateNewRandomHangmanPicture()
        {
            if (m_isGuessFromHistory)
                return;

            if (m_usedHangmanPictures.Count == TotalHangmanPictureCount)
                m_usedHangmanPictures.Clear();
            
            while (true)
            {
                var newRandom = m_randomGenerator.Next(TotalHangmanPictureCount);
                if (!m_usedHangmanPictures.Contains(newRandom))
                {
                    m_usedHangmanPictures.Add(newRandom);
                    HangmanPicture = newRandom;
                    break;
                }
            }
        }
    }
}