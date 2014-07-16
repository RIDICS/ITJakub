using Newtonsoft.Json;

namespace ITJakub.MobileApps.MainApp.JSON
{
    class GoogleIdToken
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }
    }
}
