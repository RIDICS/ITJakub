using System;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Hangman.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.View.Converter
{
    public class TotalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var progressInfo = value as ProgressInfoViewModel;
            if (progressInfo == null)
                return string.Empty;

            var lastTime = progressInfo.Time;
            var firstTime = progressInfo.FirstUpdateTime;
            var time = lastTime - firstTime;

            return string.Format("{0}:{1:00}:{2:00}", (int)time.TotalHours, time.Minutes, time.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}