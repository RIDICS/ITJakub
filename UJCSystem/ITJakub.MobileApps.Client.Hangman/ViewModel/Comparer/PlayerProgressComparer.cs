using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel.Comparer
{
    public class PlayerProgressComparer : IComparer<ProgressInfoViewModel>
    {
        public int Compare(ProgressInfoViewModel x, ProgressInfoViewModel y)
        {
            if (y.LetterCount != x.LetterCount)
                return y.LetterCount - x.LetterCount;

            if (y.Lives != x.Lives)
                return y.Lives - x.Lives;

            var timeX = x.Time - x.FirstUpdateTime;
            var timeY = y.Time - y.FirstUpdateTime;

            return timeX.CompareTo(timeY);
        }
    }
}