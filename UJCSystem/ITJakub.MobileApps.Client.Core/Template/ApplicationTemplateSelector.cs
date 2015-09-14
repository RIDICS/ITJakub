using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Core.Template
{
    public class ApplicationTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> m_applicationTemplates = new Dictionary<Type, DataTemplate>();

        public ApplicationTemplateSelector()
        {
            LoadAllDataTemplates();
        }

        private void LoadAllDataTemplates()
        {
            var templateCreator = new DataTemplateCreator();
            var apps = ApplicationLoader.Instance.GetAllApplications();
            foreach (var app in apps.Select(keyValuePair => keyValuePair.Value))
            {
                m_applicationTemplates.Add(app.ApplicationViewModel.GetType(), templateCreator.CreateDataTemplate(app.ApplicationDataTemplate));
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null && m_applicationTemplates.ContainsKey(item.GetType()))
                return m_applicationTemplates[item.GetType()];

            return base.SelectTemplateCore(item, container);
        }
    }
}