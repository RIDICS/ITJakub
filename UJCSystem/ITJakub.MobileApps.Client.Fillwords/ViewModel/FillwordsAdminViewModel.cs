using System.Collections.Generic;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class FillwordsAdminViewModel : AdminBaseViewModel
    {
        public FillwordsAdminViewModel(FillwordsDataService dataService)
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