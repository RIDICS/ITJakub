using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ITJakub.BatchImport.Client.BusinessLogic;

namespace ITJakub.BatchImport.Client.View.Converters
{
    [ValueConversion(typeof(FileStateType), typeof(Visual))]
    public class FileStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(Canvas))
            //    throw new InvalidOperationException("The target must be a Visual");

            var fileState = value as FileStateType? ?? FileStateType.Pending;

            switch (fileState)
            {

                case FileStateType.Pending:
                    return PendingIcon;

                case FileStateType.Uploading:
                    return UploadingIcon;

                case FileStateType.Processing:
                    return ProcessingIcon;

                case FileStateType.Done:
                    return DoneIcon;

                case FileStateType.Error:
                    return ErrorIcon;

            }

            return fileState == FileStateType.Processing || fileState == FileStateType.Uploading ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public Canvas PendingIcon { get; set; }        

        public Canvas UploadingIcon{ get; set; }

        public Canvas ProcessingIcon{ get; set; }

        public Canvas DoneIcon{ get; set; }
        public Canvas ErrorIcon { get; set; }
    }
}