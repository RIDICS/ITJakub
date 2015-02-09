using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class GuessViewModel
    {
        public char Letter { get; set; }

        public UserInfo Author { get; set; }

        public int WordOrder { get; set; }
    }
}