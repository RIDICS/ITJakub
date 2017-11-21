using System;

namespace Vokabular.MainService.DataContracts.Contracts.Favorite
{
    public class FavoriteLabelContractBase
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }
    }

    public class FavoriteLabelContract : FavoriteLabelContractBase
    {
        public DateTime? LastUseTime { get; set; }

        public bool IsDefault { get; set; }
    }
}
