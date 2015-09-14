using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Core.Template
{
    public class AdminTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> m_templates = new Dictionary<Type, DataTemplate>();

        public AdminTemplateSelector()
        {
            LoadAllDataTemplates();
        }

        private void LoadAllDataTemplates()
        {
            var templateCreator = new DataTemplateCreator();
            var apps = ApplicationLoader.Instance.GetAllApplications();
            foreach (var app in apps.Select(keyValuePair => keyValuePair.Value).Where(appInfo => appInfo.AdminDataTemplate != null))
            {
                m_templates.Add(app.AdminViewModel.GetType(), templateCreator.CreateDataTemplate(app.AdminDataTemplate));
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null && m_templates.ContainsKey(item.GetType()))
                return m_templates[item.GetType()];

            return base.SelectTemplateCore(item, container);
        }
    }
}