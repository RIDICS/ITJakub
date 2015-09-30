using System.Collections.Generic;
using ITJakub.MobileApps.Client.Fillwords2.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class FillwordsViewModel : ApplicationBaseViewModel
    {
        public FillwordsViewModel(FillwordsDataService dataService)
        {
            throw new System.NotImplementedException();
        }

        public override void InitializeCommunication(bool isUserOwner)
        {
            throw new System.NotImplementedException();
        }

        public override void SetTask(string data)
        {
            throw new System.NotImplementedException();
        }

        public override void EvaluateAndShowResults()
        {
            throw new System.NotImplementedException();
        }

        public override void StopCommunication()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<ActionViewModel> ActionsWithUsers { get; }
    }
}
