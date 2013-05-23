using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.Controls.FlyoutControls;
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
    /// Page that shows form to edit users data.
    /// </summary>
    public sealed partial class EditSelectedUser : Page {

        /// <summary>
        /// The edited user
        /// </summary>
        User editedUser = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditSelectedUser"/> class.
        /// </summary>
        public EditSelectedUser() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            editedUser = (User)e.Parameter;

            checkUserData(editedUser);
            
            yearComboBox.ItemsSource = Generator.getPastYears(150);
            monthComboBox.ItemsSource = Generator.getMonths();

            yearOfGraduationComboBox.ItemsSource = Generator.getNextYears(6);


            string[] date = editedUser.DateOfBirth.Split('.');
            yearComboBox.SelectedItem = int.Parse(date[2]);
            monthComboBox.SelectedItem = int.Parse(date[1]);

            int selectedMonth = int.Parse(monthComboBox.SelectedItem.ToString());
            int selectedYear = int.Parse(yearComboBox.SelectedItem.ToString());

            dayComboBox.ItemsSource = Generator.getDaysInMonth(selectedMonth, selectedYear);
            dayComboBox.SelectedItem = int.Parse(date[0]);

            classNameTextBox.Text = editedUser.ClassName;
            emailTextBox.Text = editedUser.Email;
            nameTextBox.Text = editedUser.FirstName;
            lastNameTextBox.Text = editedUser.LastName;
            nicknameTextBox.Text = editedUser.Nickname;
            usernameTextBox.Text = editedUser.Username;
            yearOfGraduationComboBox.SelectedItem = editedUser.YearOfGraduation;
            
            roleComboBox.ItemsSource = Generator.getRoles();
            if (editedUser.Role == (int)UserRole.Student) {
                roleComboBox.SelectedIndex = 1;
            }
            if (editedUser.Role == (int)UserRole.Teacher) {
                roleComboBox.SelectedIndex = 0;
            }
            if (editedUser.Role == (int)UserRole.Principal) {
                roleComboBox.SelectedIndex = 2;
            }
        }

        /// <summary>
        /// Checks the user data.
        /// </summary>
        /// <param name="u">The u.</param>
        private void checkUserData(User u) {
            if (u.ClassName == null) {
                u.ClassName = "";
            }
            if (u.ClassTeacher == null) {
                u.ClassTeacher = "";
            }
            if (u.DateOfBirth == null) {
                u.DateOfBirth = DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year;
            }
            if (u.Email == null) {
                u.Email = "";
            }
            if (u.FirstName == null) {
                u.FirstName = "";
            }
            if (u.LastName == null) {
                u.LastName = "";
            }
            if (u.Nickname == null) {
                u.Nickname = "";
            }
            if (u.OpenId == null) {
                u.OpenId = "";
            }
            if (u.Password == null) {
                u.Password = "";
            }
            if (u.ServiceName == null) {
                u.ServiceName = "";
            }
            if (u.Username == null) {
                u.Username = "";
            }
            if (u.YearOfGraduation == 0) {
                u.YearOfGraduation = DateTime.Now.Year;
            }
        }


        /// <summary>
        /// Handles the SelectionChanged event of the yearComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (yearComboBox.SelectedItem != null && monthComboBox.SelectedItem != null) {
                int selectedMonth = int.Parse(monthComboBox.SelectedItem.ToString());
                int selectedYear = int.Parse(yearComboBox.SelectedItem.ToString());

                dayComboBox.ItemsSource = Generator.getDaysInMonth(selectedMonth, selectedYear);
                dayComboBox.SelectedItem = 1;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the monthComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void monthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (yearComboBox.SelectedItem != null && monthComboBox.SelectedItem != null) {
                int selectedMonth = int.Parse(monthComboBox.SelectedItem.ToString());
                int selectedYear = int.Parse(yearComboBox.SelectedItem.ToString());

                dayComboBox.ItemsSource = Generator.getDaysInMonth(selectedMonth, selectedYear);
                dayComboBox.SelectedItem = 1;
            }
        }

        /// <summary>
        /// Handles the Click event of the editUserButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void editUserButton_Click(object sender, RoutedEventArgs e) {
            try {
                bool usernameExists = false;
                bool emailExists = false;

                if (hasUsernameChanged()) {
                    usernameExists = await Validator.doesUsernameExists(usernameTextBox.Text.Trim());
                }
                if (hasEmailChanged()) {
                    emailExists = await Validator.doesEmailExists(emailTextBox.Text.Trim());
                }
                bool doesPasswordsMatch = Validator.doesPasswordsMatch(passwordPasswordBox.Password, passwordCheckPasswordBox.Password);
            
                bool emailAndUsernameDoesNotExist = !emailExists & !usernameExists;
                bool isDataValid = emailAndUsernameDoesNotExist & doesPasswordsMatch;

                string dateOfBirth = dayComboBox.SelectedItem.ToString() + "." + monthComboBox.SelectedItem.ToString() + "." + yearComboBox.SelectedItem.ToString();
                string password = Generator.generateHashForPassword(passwordPasswordBox.Password);

                if (isDataValid) {
                    editedUser.DateOfBirth = dateOfBirth;
                    editedUser.Password = password;
                    UserTable ut = new UserTable();
                    await ut.updateUser(editedUser);
                    //TODO: Notify user that user has been edited.
                    userEdited();
                    return;
                }
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout(
                    "Chyba zadání",
                    "Data která jste zadali nemohou být uložena do databáze.\r\nZkontrolujte prosím zadání.",
                    f
                    );
                f.PlacementTarget = this.Frame;
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                } catch (Exception ex) {
                    object o = ex;
            }
        }

        /// <summary>
        /// Users has been edited.
        /// </summary>
        private void userEdited() {
            this.Frame.Navigate(typeof(EditUser));
        }

        /// <summary>
        /// Handles the TextChanged event of the nameTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            editedUser.FirstName = nameTextBox.Text;
        }

        /// <summary>
        /// Handles the TextChanged event of the lastNameTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void lastNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            editedUser.LastName = lastNameTextBox.Text;
        }

        /// <summary>
        /// Handles the TextChanged event of the nicknameTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void nicknameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            editedUser.Nickname = nicknameTextBox.Text;
        }

        /// <summary>
        /// Handles the TextChanged event of the classNameTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void classNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            editedUser.ClassName = classNameTextBox.Text;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the yearOfGraduationComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void yearOfGraduationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            editedUser.YearOfGraduation = int.Parse(yearOfGraduationComboBox.SelectedItem.ToString());
        }

        /// <summary>
        /// Determines whether [has username changed].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has username changed]; otherwise, <c>false</c>.
        /// </returns>
        private bool hasUsernameChanged() {
            if (usernameTextBox.Text.Trim() != editedUser.Username.Trim()) {
                editedUser.Username = usernameTextBox.Text;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [has email changed].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has email changed]; otherwise, <c>false</c>.
        /// </returns>
        private bool hasEmailChanged() {
            if (emailTextBox.Text.Trim() != editedUser.Email.Trim()) {
                editedUser.Email = emailTextBox.Text.Trim();
                return true;
            }
            return false;
        }

    }
}
