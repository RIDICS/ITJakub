using System;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class ProgressUpdateViewModel
    {
        public string FilledWord { get; set; }

        public int RowIndex { get; set; }

        public bool IsCorrect { get; set; }

        public UserInfo UserInfo { get; set; }

        public DateTime Time { get; set; }
    }
}
