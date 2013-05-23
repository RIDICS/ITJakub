using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using Windows.UI.Popups;
using WinRTXamlToolkit.Controls;

namespace IT_Jakub.Classes.Utils {

    /// <summary>
    /// Class that shows user specified dialogs.
    /// </summary>
    static class MyDialogs {

        /// <summary>
        /// Shows the login dialog and returns credentials.
        /// </summary>
        /// <param name="isCredentialsCorrect">if set to <c>true</c> [is credentials correct].</param>
        /// <returns></returns>
        internal static async Task<CredentialPickerResults> showLoginDialog(bool isCredentialsCorrect) {
            CredentialPickerOptions credPickerOptions = new CredentialPickerOptions();
            credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Basic;
            credPickerOptions.Message = "Pro používání této aplikace je nutné se přihlásit.\r\nPřihlaste se pomocí uživatelského jména a hesla níže.";
            credPickerOptions.Caption = "Přihlášení";
            credPickerOptions.TargetName = "IT_Jakub";
            
            credPickerOptions.AlwaysDisplayDialog = false;
            if (!isCredentialsCorrect) {
                credPickerOptions.ErrorCode = 1326;
            }

            credPickerOptions.CredentialSaveOption = CredentialSaveOption.Hidden;
            CredentialPickerResults result = await CredentialPicker.PickAsync(credPickerOptions);
            return result;
        }
    }
}
