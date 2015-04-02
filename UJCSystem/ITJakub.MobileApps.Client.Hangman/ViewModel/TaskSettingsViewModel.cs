namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class TaskSettingsViewModel
    {
        public bool GuessHistoryVisible { get; set; }

        public bool OpponentProgressVisible { get; set; }

        public char[] SpecialLetters { get; set; }
    }
}