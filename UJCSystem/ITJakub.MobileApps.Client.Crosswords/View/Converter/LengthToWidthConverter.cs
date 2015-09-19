using System;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ITJakub.MobileApps.Client.Crosswords.View.Converter
{
    public class LengthToWidthConverter :IValueConverter
    {
        private const int KeyboardButtonWidth = 60;
        private readonly Grid m_referenceGrid;
        
        public LengthToWidthConverter()
        {
            m_referenceGrid = new Grid
            {
                Height = 10
            };
        }

        private double GetActualWidth(double width)
        {
            if (m_referenceGrid.Width != width)
            {
                m_referenceGrid.Width = width;
                m_referenceGrid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return m_referenceGrid.DesiredSize.Width;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            var integer = (int) value;
            var parameterValue = double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);
            var multiplier = GetActualWidth(parameterValue);

            return integer*multiplier + 10 + KeyboardButtonWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}
