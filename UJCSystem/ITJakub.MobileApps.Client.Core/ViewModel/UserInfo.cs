using System;
using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.Core.ViewModel
{
    public class UserInfo
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long UserId { get; set; }
        public string CommunicationToken { get; set; }
        public DateTime EstimatedExpirationTime { get; set; }
        public ImageSource UserAvatar { get; set; }
    }
}