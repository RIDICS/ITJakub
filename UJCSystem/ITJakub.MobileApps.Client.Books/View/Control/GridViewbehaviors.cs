using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public static class GridViewBehaviors
    {
        public static readonly DependencyProperty IsScrollToSelectionProperty =
            DependencyProperty.RegisterAttached("IsScrollToSelection", typeof (bool),
                typeof (GridViewBehaviors), new PropertyMetadata(false, OnIsScrollToSelection));

        private static void OnIsScrollToSelection(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridView gridView = (GridView) d;
            var newValue = (bool) e.NewValue;

            if (newValue)
            {
                gridView.SelectionChanged += OnSelectionChanged;
            }
            else
            {
                gridView.SelectionChanged -= OnSelectionChanged;
            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gridView = (GridView) sender;

            if (e.AddedItems.Count == 0)
                return;

            var selectedItem = e.AddedItems[0];

            gridView.UpdateLayout();
            gridView.ScrollIntoView(selectedItem);
        }

        public static void SetIsScrollToSelection(DependencyObject d, bool value)
        {
            d.SetValue(IsScrollToSelectionProperty, value);
        }

        public static bool GetIsScrollToSelection(DependencyObject d)
        {
            return (bool)d.GetValue(IsScrollToSelectionProperty);
        }
    }
}
