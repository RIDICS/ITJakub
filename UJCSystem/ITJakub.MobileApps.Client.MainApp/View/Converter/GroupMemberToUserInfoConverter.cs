using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupMemberToUserInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var groupMember = value as GroupMemberViewModel;

            if (groupMember == null)
                return null;

            return new UserInfo
            {
                FirstName = groupMember.FirstName,
                LastName = groupMember.LastName,
                Id = groupMember.Id
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}