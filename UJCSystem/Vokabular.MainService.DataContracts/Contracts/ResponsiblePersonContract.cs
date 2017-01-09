using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResponsiblePersonContract
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class NewResponsiblePersonContract : ResponsiblePersonContract
    {
        public IList<int> ResponsibleTypeIdList { get; set; }
    }
}