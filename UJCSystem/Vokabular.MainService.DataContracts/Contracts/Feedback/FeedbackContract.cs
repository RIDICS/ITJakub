using System;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts.Feedback
{
    public class FeedbackContract
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public DateTime CreateTime { get; set; }

        public UserContract AuthorUser { get; set; }

        public string AuthorName { get; set; }

        public string AuthorEmail { get; set; }

        public FeedbackCategoryEnumContract FeedbackCategory { get; set; }

        public FeedbackTypeEnumContract FeedbackType { get; set; }

        public HeadwordContract HeadwordInfo { get; set; }

        public ProjectContract ProjectInfo { get; set; }
    }

    public class CreateFeedbackContract
    {
        string Text { get; set; }

        FeedbackCategoryEnumContract FeedbackCategory { get; set; }
    }

    public class CreateAnonymousFeedbackContract : CreateFeedbackContract
    {
        string AuthorName { get; set; }

        string AuthorEmail { get; set; }
    }
}
