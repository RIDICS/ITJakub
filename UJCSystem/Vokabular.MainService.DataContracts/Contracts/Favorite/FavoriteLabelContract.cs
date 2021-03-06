﻿using System;
using System.Collections.Generic;

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

    public class FavoriteLabelWithBooksAndCategories : FavoriteLabelContract
    {
        public List<long> ProjectIdList { get; set; }

        public List<int> CategoryIdList { get; set; }
    }
}
