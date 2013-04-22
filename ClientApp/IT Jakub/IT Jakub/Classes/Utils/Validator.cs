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
    static class Validator {


        internal async static Task<bool> doesUsernameExists(string username) {
            UserTable userTable = new UserTable();
            User u = await userTable.getUserByUsername(username);
            if (u != null) {
                return true;
            }
            return false;
        }

        internal async static Task<bool> doesEmailExists(string email) {
            UserTable userTable = new UserTable();
            User u = await userTable.getUserByEmail(email);
            if (u != null) {
                return true;
            }
            return false;
        }

        internal static bool doesPasswordsMatch(string password, string passwordCheck) {
            if (password == passwordCheck) {
                return true;
            }
            return false;
        }
    }
}
