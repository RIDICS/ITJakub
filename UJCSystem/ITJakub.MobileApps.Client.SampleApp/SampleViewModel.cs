using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.SampleApp
{
    public class SampleViewModel : ApplicationBaseViewModel
    {
        private string m_testString;

        public SampleViewModel()
        {
            //TODO tady nacist data pro aplikaci z dataservice apod.
            TestString = "Testovaci string z viewModelu aplikace";
        }

        public string TestString
        {
            get { return m_testString; }
            set { m_testString = value; RaisePropertyChanged();}
        }
    }
}
