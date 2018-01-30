using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    public class AddBookToUserGroupRequestContract
    {
        public IList<long> BookIdList { get; set; }
    }
}