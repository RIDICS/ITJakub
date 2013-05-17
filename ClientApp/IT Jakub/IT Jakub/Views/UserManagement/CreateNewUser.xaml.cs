using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Exceptions;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;

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
    public sealed partial class CreateNewUser : Page {
        public CreateNewUser() {
            this.InitializeComponent();
            yearComboBox.ItemsSource = Generator.getPastYears(150);
            monthComboBox.ItemsSource = Generator.getMonths();
            monthComboBox.SelectedItem = DateTime.Now.Month;
            yearComboBox.SelectedItem = DateTime.Now.Year;

            int selectedMonth = int.Parse(monthComboBox.SelectedItem.ToString());
            int selectedYear = int.Parse(yearComboBox.SelectedItem.ToString());

            dayComboBox.ItemsSource = Generator.getDaysInMonth(selectedMonth,selectedYear);
            dayComboBox.SelectedItem = DateTime.Now.Day;

            yearOfGraduationComboBox.ItemsSource = Generator.getNextYears(6);
            yearOfGraduationComboBox.SelectedItem = DateTime.Now.Year + 4;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        private void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (yearComboBox.SelectedItem != null && monthComboBox.SelectedItem != null) {
                int selectedMonth = int.Parse(monthComboBox.SelectedItem.ToString());
                int selectedYear = int.Parse(yearComboBox.SelectedItem.ToString());

                dayComboBox.ItemsSource = Generator.getDaysInMonth(selectedMonth, selectedYear);
                dayComboBox.SelectedItem = 1;
            }
        }

        private void monthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (yearComboBox.SelectedItem != null && monthComboBox.SelectedItem != null) {
                int selectedMonth = int.Parse(monthComboBox.SelectedItem.ToString());
                int selectedYear = int.Parse(yearComboBox.SelectedItem.ToString());

                dayComboBox.ItemsSource = Generator.getDaysInMonth(selectedMonth, selectedYear);
                dayComboBox.SelectedItem = 1;
            }
        }

        private async void createUserButton_Click(object sender, RoutedEventArgs e) {
            try {
                bool usernameExists = await Validator.doesUsernameExists(usernameTextBox.Text.Trim());
                bool emailExists = await Validator.doesEmailExists(emailTextBox.Text.Trim());
                bool doesPasswordsMatch = Validator.doesPasswordsMatch(passwordPasswordBox.Password, passwordCheckPasswordBox.Password);
            
                bool emailAndUsernameDoesNotExist = !emailExists & !usernameExists;
                bool isDataValid = emailAndUsernameDoesNotExist & doesPasswordsMatch;

                string dateOfBirth = dayComboBox.SelectedItem.ToString() + "." + monthComboBox.SelectedItem.ToString() + "." + yearComboBox.SelectedItem.ToString();
                string password = Generator.generateHashForPassword(passwordPasswordBox.Password);

                if (isDataValid) {
                    User u = new User {
                        ClassName = classNameTextBox.Text,
                        ClassTeacher = "",
                        DateOfBirth = dateOfBirth,
                        Email = emailTextBox.Text.Trim(),
                        FirstName = nameTextBox.Text,
                        LastName = lastNameTextBox.Text,
                        Nickname = nicknameTextBox.Text,
                        OpenId = "",
                        ServiceName = "",
                        Password = password,
                        Username = usernameTextBox.Text,
                        YearOfGraduation = int.Parse(yearOfGraduationComboBox.SelectedItem.ToString())
                    };
                    UserTable ut = new UserTable();
                    await ut.createUser(u);
                    MyDialogs.showDialogOK("Uživatel byl vytvořen", userCreated);
                }
            } catch (Exception ex) {
                MyDialogs.showDialogOK(ex.Message);
                // Funguje az na druhe kliknuti
                // Pokud se uzivatel znovu pripoji tak se uzivatel vytvori :(
            }
        }
            
        

        private void userCreated(IUICommand command) {
            if (this.Frame.CanGoBack) {
                this.Frame.GoBack();
            } else {
                this.Frame.Navigate(typeof(ApplicationStart.ApplicationStart));
            }
        }

        
        
    }
}
