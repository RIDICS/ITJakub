using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class UserResultViewModel
    {
        public AuthorInfo UserInfo { get; set; }

        public double PercentageSuccess
        {
            get { return (double) CorrectAnswers/TotalAnswers*100; }
        }

        public int CorrectAnswers { get; set; }

        public int TotalAnswers { get; set; }
    }
}