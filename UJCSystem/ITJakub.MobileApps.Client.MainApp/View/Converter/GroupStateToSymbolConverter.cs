using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupStateToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Symbol))
                throw new InvalidOperationException("The target must be a Symbol");

            var groupState = (GroupStateContract)value;

            switch (groupState)
            {
                case GroupStateContract.Created:
                    return Symbol.SolidStar;
                case GroupStateContract.AcceptMembers:
                    return Symbol.AddFriend;
                case GroupStateContract.WaitingForStart:
                    return Symbol.Clock;
                case GroupStateContract.Running:
                    return Symbol.Play;
                case GroupStateContract.Paused:
                    return Symbol.Pause;
                case GroupStateContract.Closed:
                    return Symbol.Stop;
                default:
                    return Symbol.Placeholder;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}