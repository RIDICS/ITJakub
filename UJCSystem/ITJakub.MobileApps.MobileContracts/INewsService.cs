using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.MobileApps.MobileContracts.News;

namespace ITJakub.MobileApps.MobileContracts
{
    [ServiceContract]
    public interface INewsService
    {
        [OperationContract]
        IList<NewsSyndicationItemContract> GetNewsForMobileApps(int start, int count);
    }
}