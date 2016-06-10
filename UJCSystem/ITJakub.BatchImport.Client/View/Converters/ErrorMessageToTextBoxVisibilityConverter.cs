using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ITJakub.BatchImport.Client.BusinessLogic;

namespace ITJakub.BatchImport.Client.View.Converters
{
    [ValueConversion(typeof(String), typeof(Visibility))]
    public class ErrorMessageToTextBoxVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");

            var errorMessage = value as string;

            return string.IsNullOrEmpty(errorMessage) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}