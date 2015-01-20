using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Core.Manager.Groups;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupStateToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Symbol))
                throw new InvalidOperationException("The target must be a Symbol");

            var groupState = (GroupState)value;

            switch (groupState)
            {
                case GroupState.Created:
                    return Symbol.Placeholder;
                case GroupState.AcceptMembers:
                    return Symbol.AddFriend;
                case GroupState.WaitingForStart:
                    return Symbol.Clock;
                case GroupState.Running:
                    return Symbol.Play;
                case GroupState.Paused:
                    return Symbol.Pause;
                case GroupState.Closed:
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