using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
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

namespace IT_Jakub.Views.ApplicationStart {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class UserManagement : Page {

        private LinkedList<User> checkedUserList = new LinkedList<User>();

        public UserManagement() {
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
            Users u = new Users();
            userList.ItemsSource = u.getAllUsers();
        }

        private async void removeUser_Click(object sender, RoutedEventArgs e) {
            ItemCollection items = userList.Items;
            Users us = new Users();
            for (LinkedListNode<User> node = checkedUserList.First; node != checkedUserList.Last.Next; node = node.Next) {
                User u = node.Value;
                await us.deleteUser(u);
            }
            checkedUserList.Clear();
            userList.ItemsSource = us.getAllUsers();
        }

        /**
         * Stare metody pro checkbox, nyni se vyuziva listView
         * 
         * 
        private void UserCheckBox_Checked(object sender, RoutedEventArgs e) {
            CheckBox cb = (CheckBox)sender;
            User u = cb.DataContext as User;
            checkedUserList.AddFirst(u);
        }

        private void UserCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            CheckBox cb = (CheckBox)sender;
            User u = cb.DataContext as User;
            checkedUserList.Remove(u);
        }
        
         */

        private async void createUserButton_Click(object sender, RoutedEventArgs e) {
            MobileService ms = MobileService.getInstance();
            Users us = new Users();
            User u = new User { Username = "newUser", Password = "passwd" };
            await us.createUser(u);
            userList.ItemsSource = us.getAllUsers();
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
    }
}
