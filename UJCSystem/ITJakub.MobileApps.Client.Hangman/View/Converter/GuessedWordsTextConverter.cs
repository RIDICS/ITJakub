using System;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Hangman.View.Converter
{
    public class GuessedWordsTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var count = (int)value;
            if (count == 1)
                return "uhodnut� slovo";

            if (count > 1 && count <= 4)
                return "uhodnut� slova";

            return "uhodnut�ch slov";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}