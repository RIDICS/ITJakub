using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.Crosswords.View.Converter
{
    public class AnswerToCellBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush");

            var answerIsCorrect = (bool?)value;

            if (!answerIsCorrect.HasValue)
                return new SolidColorBrush(Colors.Transparent);
            
            return answerIsCorrect.Value
                ? new SolidColorBrush(Color.FromArgb(196, 50, 205, 50))
                : new SolidColorBrush(Color.FromArgb(196, 139, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}