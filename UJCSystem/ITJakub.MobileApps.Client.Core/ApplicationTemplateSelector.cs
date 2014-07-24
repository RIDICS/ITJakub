﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ITJakub.MobileApps.Client.Core
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
            var apps = ApplicationLoader.Instance.GetAllApplications();
            foreach (var app in apps.Select(keyValuePair => keyValuePair.Value))
            {
                m_applicationTemplates.Add(app.ApplicationViewModel.GetType(), CreateDataTemplate(app.ApplicationDataTemplate));
            }
        }

        private DataTemplate CreateDataTemplate(Type viewType)
        {
            var dataTemplate = CreateDataTemplate(viewType.Namespace, viewType.Name);
            return dataTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null && m_applicationTemplates.ContainsKey(item.GetType()))
                return m_applicationTemplates[item.GetType()];

            return base.SelectTemplateCore(item, container);
        }

        private DataTemplate CreateDataTemplate(string userControlNamespace, string viewName)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format(@"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
            xmlns:plugin='using:{0}'>", userControlNamespace));

            sb.Append(string.Format(@"<plugin:{0} />", viewName));            

            sb.Append(@"</DataTemplate>");

            var itemsTemplate = XamlReader.Load(sb.ToString()) as DataTemplate;
            return itemsTemplate;
        }
    }
}