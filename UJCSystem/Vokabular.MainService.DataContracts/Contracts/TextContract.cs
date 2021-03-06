﻿namespace Vokabular.MainService.DataContracts.Contracts
{
    public class TextContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }
        public long BookVersionId { get; set; }
    }

    public class FullTextContract : TextContract
    {
        public string Text { get; set; }
    }

    public class TextWithPageContract : TextContract
    {
        public PageContract ParentPage { get; set; }
    }

    public class CreateTextVersionRequestContract
    {
        public string Text { get; set; }
        public long? ResourceVersionId { get; set; }
    }

    public class CreateTextRequestContract
    {
        public string Text { get; set; }
    }
}