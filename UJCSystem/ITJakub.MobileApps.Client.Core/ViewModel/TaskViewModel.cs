using System;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.ViewModel
{
    public class TaskViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public string Data { get; set; }
        public UserInfo Author { get; set; }
        public ApplicationType Application { get; set; }
    }
}