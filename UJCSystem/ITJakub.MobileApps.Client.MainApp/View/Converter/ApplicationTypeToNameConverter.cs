using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using ITJakub.MobileApps.Client.Core;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.MainApp.View.Converter
{
    public class ApplicationTypeToNameConverter : IValueConverter
    {
        private Dictionary<ApplicationType, ApplicationBase> m_applications;

        public ApplicationTypeToNameConverter()
        {
            var dataService = Container.Current.Resolve<IDataService>();
            dataService.GetAllApplications((apps, exception) =>
            {
                if (exception != null)
                    return;

                m_applications = apps;
            });
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            var applicationType = (ApplicationType)value;

            return applicationType == ApplicationType.Unknown ? "(Není zvoleno)" : m_applications[applicationType].Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Conversion back is not supported");
        }
    }
}