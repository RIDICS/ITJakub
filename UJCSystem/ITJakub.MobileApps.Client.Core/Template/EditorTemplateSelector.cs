using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ITJakub.MobileApps.Client.Shared.Template;

namespace ITJakub.MobileApps.Client.Core.Template
{
    public class EditorTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> m_editorTemplates = new Dictionary<Type, DataTemplate>();

        public EditorTemplateSelector()
        {
            LoadAllDataTemplates();
        }

        private void LoadAllDataTemplates()
        {
            var templateCreator = new DataTemplateCreator();
            var apps = ApplicationLoader.Instance.GetAllApplications();
            foreach (var app in apps.Select(keyValuePair => keyValuePair.Value).Where(appInfo => appInfo.EditorDataTemplate != null))
            {
                m_editorTemplates.Add(app.EditorViewModel.GetType(), templateCreator.CreateDataTemplate(app.EditorDataTemplate));
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null && m_editorTemplates.ContainsKey(item.GetType()))
                return m_editorTemplates[item.GetType()];

            return base.SelectTemplateCore(item, container);
        }
    }
}