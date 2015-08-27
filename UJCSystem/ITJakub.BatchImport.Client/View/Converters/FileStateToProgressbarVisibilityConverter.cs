using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ITJakub.BatchImport.Client.BusinessLogic;

namespace ITJakub.BatchImport.Client.View.Converters
{
    [ValueConversion(typeof(FileStateType), typeof(Visibility))]
    class FileStateToProgressbarVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");

            var fileState = value as FileStateType? ?? FileStateType.Pending;

            return fileState == FileStateType.Processing || fileState == FileStateType.Uploading ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }    
}
