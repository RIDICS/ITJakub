using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Core.Manager.Groups;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupStateToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return string.Empty;

            var groupState = (GroupState)value;
            switch (groupState)
            {
                case GroupState.Created:
                    return "Vytvořená (pro ostatní skrytá)";
                case GroupState.AcceptMembers:
                    return "Přijímání členů";
                case GroupState.WaitingForStart:
                    return "Čekání na spuštění";
                case GroupState.Running:
                    return "Spuštěná";
                case GroupState.Paused:
                    return "Pozastavená";
                case GroupState.Closed:
                    return "Ukončená";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}