using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.MainApp.View.Converter;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class ApplicationTypeComparator : IComparer<ApplicationType>
    {
        private readonly ApplicationTypeToNameConverter m_appToNameConverter;

        public ApplicationTypeComparator()
        {
            m_appToNameConverter = new ApplicationTypeToNameConverter();
        }

        public int Compare(ApplicationType x, ApplicationType y)
        {
            var nameX = (string)m_appToNameConverter.Convert(x, typeof(string), null, string.Empty);
            var nameY = (string)m_appToNameConverter.Convert(y, typeof(string), null, string.Empty);
            return string.Compare(nameX, nameY, StringComparison.CurrentCulture);
        }
    }
}
