﻿using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class HangmanTaskContract
    {
        [JsonProperty("Words")]
        public string[] Words { get; set; }
    }
}