using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel.Comparer
{
    public class PlayerProgressComparer : IComparer<ProgressViewModel>
    {
        public int Compare(ProgressViewModel x, ProgressViewModel y)
        {
            if (x.CorrectAnswers != y.CorrectAnswers)
                return y.CorrectAnswers - x.CorrectAnswers;

            return x.GameTime.CompareTo(y.GameTime);
        }
    }
}