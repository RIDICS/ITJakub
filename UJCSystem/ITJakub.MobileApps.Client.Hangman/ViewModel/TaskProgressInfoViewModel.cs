using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class TaskProgressInfoViewModel
    {
        public string Word { get; set; }
        public string Hint { get; set; }
        public int Lives { get; set; }
        public int HangmanCount { get; set; }
        public bool Win { get; set; }
        public int GuessedWordCount { get; set; }
        public int GuessedLetterCount { get; set; }
        public bool IsNewWord { get; set; }
        public int HangmanPicture { get; set; }
        public IList<char> DeactivatedKeys { get; set; }
    }
}