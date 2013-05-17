using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub.Views.UserManagement {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditUser : Page {

        User selectedUser = null;
        
        public EditUser() {
            this.InitializeComponent();
            updateUserList_Click(updateUserList, null);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        private void updateUserList_Click(object sender, RoutedEventArgs e) {
            UserTable u = new UserTable();
            try {
                MobileServiceCollectionView<User> allUsers = u.getAllUsers();
                userList.ItemsSource = allUsers;
                /* Neodchycena vyjímka: 
                 * Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException was unhandled
                 * The request could not be completed.  (NameResolutionFailure)
                 * 
                 * Objevuje se při pokusu o vylistování uživatelů v offline režimu.
                 * 
                 */
            } catch (Exception ex){
                MyDialogs.showDialogOK(ex.Message);
                return;
            }
        }

        private async void removeUser_Click(object sender, RoutedEventArgs e) {
            ItemCollection items = userList.Items;
            UserTable us = new UserTable();
            await us.deleteUser(selectedUser);

            selectedUser = null;
            userList.ItemsSource = us.getAllUsers();
        }

        private void createUserButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(CreateNewUser));
        }

        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.RemovedItems.Count > 0) {
                selectedUser = null;
            }

            if (e.AddedItems.Count > 0) {
                selectedUser = (User)e.AddedItems[0];
            }
        }

        private void editUser_Click(object sender, RoutedEventArgs e) {
            if (selectedUser != null) {
                this.Frame.Navigate(typeof(EditSelectedUser), selectedUser);
            }
        } 
    }
}
