using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class GroupStateToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return string.Empty;

            var groupState = (GroupStateContract)value;
            switch (groupState)
            {
                case GroupStateContract.Created:
                    return "Vytvořená";
                case GroupStateContract.AcceptMembers:
                    return "Přijímání členů";
                case GroupStateContract.WaitingForStart:
                    return "Čekání na spuštění";
                case GroupStateContract.Running:
                    return "Spuštěná";
                case GroupStateContract.Paused:
                    return "Pozastavená";
                case GroupStateContract.Closed:
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