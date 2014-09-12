using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.DataContracts.Json
{
    public class LiveIdUserInfo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "emails")]
        public EmailClass Emails { get; set; }


        public class EmailClass
        {
            [JsonProperty(PropertyName = "preferred")]
            public string Preferred { get; set; }

            [JsonProperty(PropertyName = "account")]
            public string Account { get; set; }
        }
    }
}
