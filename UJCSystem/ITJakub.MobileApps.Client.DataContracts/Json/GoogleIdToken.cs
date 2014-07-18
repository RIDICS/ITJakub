using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.DataContracts.Json
{
    public class GoogleIdToken
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }
    }
}
