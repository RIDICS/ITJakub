using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Chat.DataContract
{
    public class ChatMessage
    {
        [JsonProperty("Text")]
        public string Text { get; set; }
    }
}
