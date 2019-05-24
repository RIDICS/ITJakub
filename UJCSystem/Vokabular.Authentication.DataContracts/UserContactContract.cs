using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Authentication.DataContracts
{
    public class UserContactContract : ContractBase
    {
        public int UserId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ContactTypeEnum ContactType { get; set; }

        public string Value { get; set; }
    }
}