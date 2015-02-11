using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataContract
{
    public class FullUpdateContract : UpdateContract
    {
        [JsonProperty("ImageCursorX")]
        public double? ImageCursorPositionX { get; set; }

        [JsonProperty("ImageCursorY")]
        public double? ImageCursorPositionY { get; set; }
    }
}
