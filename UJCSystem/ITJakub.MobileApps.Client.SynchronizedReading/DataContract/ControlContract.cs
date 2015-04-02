using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataContract
{
    public class ControlContract
    {
        [JsonProperty("ReaderUser")]
        public UserInfo ReaderUser { get; set; }

        [JsonProperty("PageId")]
        public string PageId { get; set; }


        public class UserInfo
        {
            [JsonProperty("UserId")]
            public long UserId { get; set; }

            [JsonProperty("FirstName")]
            public string FirstName { get; set; }

            [JsonProperty("LastName")]
            public string LastName { get; set; }
        }
    }
}
