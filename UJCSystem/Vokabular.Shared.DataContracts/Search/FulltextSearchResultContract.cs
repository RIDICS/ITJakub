﻿using System.Collections.Generic;

namespace Vokabular.Shared.DataContracts.Search
{
    public class FulltextSearchResultContract
    {
        public int Count { get; set; }
        public IList<long> ProjectIds { get; set; }
    }
}