using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Core
{
    public class ApplicationTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<string,DataTemplate> m_applicationTemplates = new Dictionary<string, DataTemplate>();

        public ApplicationTemplateSelector()
        {
            LoadAllDataTemplates();
        }

        private void LoadAllDataTemplates()
        {
            var apps = ApplicationLoader.Instance.GetAllApplications();
            foreach (var app in apps)
                m_applicationTemplates.Add(app.ApplicationViewModel.GetType().ToString(), app.ApplicationDataTemplate);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null && m_applicationTemplates.ContainsKey(item.GetType().ToString()))
                return m_applicationTemplates[item.GetType().ToString()];

            return base.SelectTemplateCore(item, container);
        }
    }
}
