using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel.Comparer
{
    public class PlayerRankComparer : IComparer<PlayerRankViewModel>
    {
        public int Compare(PlayerRankViewModel x, PlayerRankViewModel y)
        {
            if (x.LetterCount != y.LetterCount)
                return y.LetterCount - x.LetterCount;

            return x.GameTime.CompareTo(y.GameTime);
        }
    }
}