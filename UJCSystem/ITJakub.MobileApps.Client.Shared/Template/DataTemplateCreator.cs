using System;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace ITJakub.MobileApps.Client.Shared.Template
{
    public class DataTemplateCreator
    {
        public DataTemplate CreateDataTemplate(Type viewType)
        {
            var dataTemplate = CreateDataTemplate(viewType.Namespace, viewType.Name);
            return dataTemplate;
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