using System;
using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public abstract class TextCommentContractBase
    {
        public long Id { get; set; }

        public string TextReferenceId { get; set; }

        public string Text { get; set; }
    }

    public class CreateTextCommentContract : TextCommentContractBase
    {
        public long? ParentCommentId { get; set; }
    }

    public class GetTextCommentContract : TextCommentContractBase
    {
        public DateTime CreateTime { get; set; }

        public UserContract User { get; set; }

        public List<GetTextCommentContract> TextComments { get; set; }

        public long TextResourceId { get; set; }
    }
}