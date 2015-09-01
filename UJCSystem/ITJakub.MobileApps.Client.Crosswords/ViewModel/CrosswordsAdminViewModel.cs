using System.Collections.Generic;
using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordsAdminViewModel : AdminBaseViewModel
    {
        public CrosswordsAdminViewModel(ICrosswordsDataService dataService)
        {
        }

        public override void SetTask(string data)
        {
            throw new System.NotImplementedException();
        }

        public override void InitializeCommunication()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateGroupMembers(IEnumerable<UserInfo> members)
        {
            throw new System.NotImplementedException();
        }
    }
}