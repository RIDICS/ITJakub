namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class TaskInfoViewModel
    {
        public string Word { get; set; }
        public int Lives { get; set; }
        public bool Win { get; set; }
        public int GuessedWordCount { get; set; }
        public int GuessedLetterCount { get; set; }
        public bool IsNewWord { get; set; }
    }
}