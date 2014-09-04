using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    public class AutoScrollListView : ListView
    {
        public AutoScrollListView()
        {
            SelectionMode = ListViewSelectionMode.None;
            
            if (Items == null)
                return;

            Items.VectorChanged += ScrollToBottom;
            ScrollToBottom();
        }

        public async void ScrollToBottom()
        {
            await Task.Delay(100);
            ScrollToBottom(Items, null);
        }

        private async void ScrollToBottom(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            var selectedIndex = sender.Count - 1;
            if (selectedIndex < 0)
                return;

            SelectedIndex = selectedIndex;
            UpdateLayout();
            await Task.Delay(50);
            ScrollIntoView(SelectedItem);
        }
    }
}
