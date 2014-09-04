namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class TaskInfoViewModel
    {
        public string Word { get; set; }
        public int Lives { get; set; }
        public bool Win { get; set; }
        public int WordGuessed { get; set; }
        public bool IsNewWord { get; set; }
    }
}