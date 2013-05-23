using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace IT_Jakub.Classes.Utils {
    /// <summary>
    /// Class Validate unique database fields.
    /// </summary>
    static class Validator {


        /// <summary>
        /// Check if the username does exists in database.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        internal async static Task<bool> doesUsernameExists(string username) {
            UserTable userTable = new UserTable();
            User u = await userTable.getUserByUsername(username);
            if (u != null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the email does exists in database.
        /// </summary>
        /// <param name="username">The email.</param>
        /// <returns></returns>
        internal async static Task<bool> doesEmailExists(string email) {
            UserTable userTable = new UserTable();
            User u = await userTable.getUserByEmail(email);
            if (u != null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the password fields match.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordCheck">The password to check.</param>
        /// <returns></returns>
        internal static bool doesPasswordsMatch(string password, string passwordCheck) {
            if (password == passwordCheck) {
                return true;
            }
            return false;
        }
    }
}
