using Vokabular.MainService.DataContracts.Contracts.Feedback;

namespace ITJakub.Web.Hub.DataContracts
{
    public class FeedbackExtendedContract : FeedbackContract
    {
        public string CreateTimeString { get; set; }
    }
}