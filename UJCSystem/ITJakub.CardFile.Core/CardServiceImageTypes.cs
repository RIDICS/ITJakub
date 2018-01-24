﻿using Vokabular.Shared.DataContracts.Attribute;

namespace ITJakub.CardFile.Core
{
    public enum CardServiceImageTypes
    {
        [StringValue("full")]
        Full,

        [StringValue("preview")]
        Preview,

        [StringValue("thumbnail")]
        Thumbnail,
    }
}