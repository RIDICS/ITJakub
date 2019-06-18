using System.Collections.Generic;

namespace Vokabular.Authentication.DataContracts
{
    public class ListContract<TContract> : ContractBase
    {
        public IList<TContract> Items { get; set; }

        public int ItemsCount { get; set; }
    }
}