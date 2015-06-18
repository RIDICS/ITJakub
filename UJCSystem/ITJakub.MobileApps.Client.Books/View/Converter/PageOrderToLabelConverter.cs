using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Books.Message;
using ITJakub.MobileApps.Client.Books.ViewModel;

namespace ITJakub.MobileApps.Client.Books.View.Converter
{
    public class PageOrderToLabelConverter : IValueConverter
    {
        private ObservableCollection<PageViewModel> m_pageList;

        public PageOrderToLabelConverter()
        {
            Messenger.Default.Register<PageListMessage>(this, message =>
            {
                m_pageList = message.PageList;
            });
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var numValue = (int) ((double) value);

            if (m_pageList == null)
                return numValue;

            return m_pageList[numValue - 1].Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}