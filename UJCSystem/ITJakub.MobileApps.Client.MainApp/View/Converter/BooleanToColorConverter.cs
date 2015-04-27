using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class BooleanToColorConverter : IValueConverter
    {
        private bool m_isDefaultWhite;
        private Color m_defaultColor = Colors.Transparent;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush");

            var isTrue = (bool) value;
            var newColor = parameter != null ? (Color) parameter : Colors.Lime;

            return new SolidColorBrush(isTrue ? newColor : m_defaultColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }

        public bool IsDefaultWhite
        {
            get { return m_isDefaultWhite; }
            set
            {
                m_isDefaultWhite = value;
                m_defaultColor = value ? Colors.White : Colors.Transparent;
            }
        }
    }
}