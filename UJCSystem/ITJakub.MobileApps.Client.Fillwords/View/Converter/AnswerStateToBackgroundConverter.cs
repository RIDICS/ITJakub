using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using ITJakub.MobileApps.Client.Fillwords.ViewModel.Enum;

namespace ITJakub.MobileApps.Client.Fillwords.View.Converter
{
    public class AnswerStateToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush");

            var answerState = (AnswerState) value;

            switch (answerState)
            {
                case AnswerState.Correct:
                    return new SolidColorBrush(Color.FromArgb(255, 50, 205, 50));
                case AnswerState.Incorrect:
                    return new SolidColorBrush(Color.FromArgb(255, 230, 100, 100));
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}