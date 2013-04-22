using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Exceptions;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using IT_Jakub.Classes.Utils;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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

    public sealed partial class UserManagementStart : Page {

        private LinkedList<User> checkedUserList = new LinkedList<User>();

        public UserManagementStart() {
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
            for (LinkedListNode<User> node = checkedUserList.First; node != checkedUserList.Last.Next; node = node.Next) {
                User u = node.Value;
                await us.deleteUser(u);
            }
            checkedUserList.Clear();
            userList.ItemsSource = us.getAllUsers();
        }

        private void createUserButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(CreateNewUser));
        }

        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                for (int i = 0; i < e.AddedItems.Count; i++) {
                    User u = (User)e.AddedItems[i];
                    checkedUserList.AddFirst(u);
                }
            }
            if (e.RemovedItems.Count > 0) {
                for (int i = 0; i < e.RemovedItems.Count; i++) {
                    User u = (User)e.RemovedItems[i];
                    checkedUserList.Remove(u);
                }
            }
        }

        private void editUser_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(EditUser));
        }        
    }
}
