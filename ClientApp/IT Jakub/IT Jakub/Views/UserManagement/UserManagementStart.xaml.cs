using IT_Jakub.Classes.DatabaseModels;
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
    /// Page that shows userList of users in database.
    /// </summary>

    public sealed partial class UserManagementStart : Page {

        /// <summary>
        /// The checked user list
        /// </summary>
        private LinkedList<User> checkedUserList = new LinkedList<User>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementStart"/> class.
        /// </summary>
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

        /// <summary>
        /// Handles the Click event of the updateUserList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void updateUserList_Click(object sender, RoutedEventArgs e) {
            UserTable u = new UserTable();
            try {
                List<User> allUsers = await u.getAllUsers();
                userList.ItemsSource = allUsers;
            } catch (Exception ex){
                object o = ex;
                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the removeUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the createUserButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void createUserButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(CreateNewUser));
        }

        /// <summary>
        /// Handles the SelectionChanged event of the userList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the editUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void editUser_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(EditUser));
        }        
    }
}
