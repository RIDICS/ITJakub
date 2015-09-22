using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class UserDetailContract: UserContract
    {
        public IList<GroupContract> Groups { get; set; }
    }
}