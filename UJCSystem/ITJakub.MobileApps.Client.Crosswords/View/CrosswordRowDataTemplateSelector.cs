using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ITJakub.MobileApps.Client.Crosswords.ViewModel;

namespace ITJakub.MobileApps.Client.Crosswords.View
{
    public class CrosswordRowDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RowDataTemplate { get; set; }
        public DataTemplate SpaceDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var rowViewModel = item as CrosswordRowViewModel;
            if (rowViewModel == null)
                return null;

            return rowViewModel.Cells != null ? RowDataTemplate : SpaceDataTemplate;
        }
    }
}
