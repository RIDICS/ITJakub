using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ITJakubServerInterface;

namespace ITJakub
{
    public sealed partial class MainPage : Page
    {
        // Hlavni objekt, pres ktery se pripojis do databaze, zaklada se jenom jednou pro aplikaci; druhy parametr je aplikacni heslo
        public static MobileServiceClient MobileService = new MobileServiceClient("https://itjakub.azure-mobile.net/",
                                                                                  "IKzmwpfkbiryIglFPmMRlsmAqwnLdY61");

        // MobileServiceCollectionView implements ICollectionView (useful for databinding to lists) and
        // is integrated with your Mobile Service to make it easy to bind your data to the ListView
        private MobileServiceCollectionView<Session> items;

        //takhle si založís objekt pro přístup k databázové tabulce, všechny operace pak děláš přes ní
        private IMobileServiceTable<Session> sessionTable = MobileService.GetTable<Session>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void CreateSession(Session session)
        {
            await sessionTable.InsertAsync(session);
            items.Add(session);
        }

        //Načte seznam vytvořených realcí
        private void RefreshSessions()
        {
            //Pokud odkomentuje radek nize, odfiltrujes relace s nastavenym heslem
            items = sessionTable
                /*       .Where(sessionItem => sessionItem.Password == "") */
                .ToCollectionView();
            ListItems.ItemsSource = items;
        }

        private async void DeleteSession(Session item)
        {
            await sessionTable.DeleteAsync(item);
            items.Remove(item);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshSessions();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new Session { Name = TextInput.Text, Password = "" };
            CreateSession(todoItem);
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Session item = cb.DataContext as Session;
            DeleteSession(item);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshSessions();
        }

        private async void FacebookButton_Click(object sender, RoutedEventArgs e)
        {
            await Authenticate(MobileServiceAuthenticationProvider.Facebook);
        }

        private async void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            await Authenticate(MobileServiceAuthenticationProvider.Google);
        }

        private async void LiveButton_Click(object sender, RoutedEventArgs e)
        {
            await Authenticate(MobileServiceAuthenticationProvider.MicrosoftAccount);
        }

        private async Task Authenticate(MobileServiceAuthenticationProvider provider)
        {
            string message;
            try
            {
                var user = await MobileService.LoginAsync(provider);
                message = string.Format("Nyní jste přihlášen(a) jako - {0}", user.UserId);
            }
            catch (InvalidOperationException)
            {
                message = "Přihlášení se nezdařilo";
            }

            var dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand("OK"));
            await dialog.ShowAsync();
        }
    }
}