using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ITJakub.MobileApps.Client.Shared.Template;

namespace ITJakub.MobileApps.Client.Core.Template
{
    public class TaskPreviewTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> m_templates = new Dictionary<Type, DataTemplate>();

        public TaskPreviewTemplateSelector()
        {
            LoadAllDataTemplates();
        }

        private void LoadAllDataTemplates()
        {
            var templateCreator = new DataTemplateCreator();
            var apps = ApplicationLoader.Instance.GetAllApplications();
            foreach (var app in apps.Select(keyValuePair => keyValuePair.Value).Where(appInfo => appInfo.TaskPreviewDataTemplate != null))
            {
                m_templates.Add(app.TaskPreviewViewModel.GetType(), templateCreator.CreateDataTemplate(app.TaskPreviewDataTemplate));
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