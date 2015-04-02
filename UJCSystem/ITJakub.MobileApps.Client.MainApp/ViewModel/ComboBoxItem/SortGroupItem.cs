namespace ITJakub.MobileApps.Client.MainApp.ViewModel.ComboBoxItem
{
    public class SortGroupItem
    {
        public string Name { get; set; }

        public SortType Type { get; set; }

        public enum SortType
        {
            Name, CreateTime, State
        }
    }
}
