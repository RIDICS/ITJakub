using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    public class AddBookToRoleRequestContract
    {
        public IList<long> BookIdList { get; set; }
    }
}