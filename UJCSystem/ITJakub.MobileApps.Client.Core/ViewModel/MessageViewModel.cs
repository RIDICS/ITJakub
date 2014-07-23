using System;

namespace ITJakub.MobileApps.Client.Core.ViewModel
{
    public class MessageViewModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsMyMessage { get; set; }
    }
}