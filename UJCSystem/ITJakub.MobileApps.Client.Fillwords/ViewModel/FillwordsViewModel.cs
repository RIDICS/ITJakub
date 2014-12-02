using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class FillwordsViewModel : ApplicationBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;

        public FillwordsViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;
        }

        public override void InitializeCommunication()
        {
            throw new System.NotImplementedException();
        }

        public override void SetTask(string data)
        {
            throw new System.NotImplementedException();
        }

        public override void StopCommunication()
        {
            throw new System.NotImplementedException();
        }
    }
}
