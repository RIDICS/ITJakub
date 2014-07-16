using Newtonsoft.Json;

namespace ITJakub.MobileApps.MainApp.JSON
{
    public class GoogleUserInfo
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        
        [JsonProperty("name")]
        public NameClass Name { get; set; }

        public class NameClass
        {
            [JsonProperty("familyName")]
            public string FamilyName { get; set; }

            [JsonProperty("givenName")]
            public string GivenName { get; set; }
        }
    }
}