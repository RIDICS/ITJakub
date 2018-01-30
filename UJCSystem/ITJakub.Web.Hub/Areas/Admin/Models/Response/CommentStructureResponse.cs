using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Response
{
    public class CommentStructureResponse : TextCommentContractBase
    {
        public string Picture { get; set; }
        public bool Nested { get; set; }
        public long TextId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Order { get; set; }
        public long Time { get; set; }
    }
}