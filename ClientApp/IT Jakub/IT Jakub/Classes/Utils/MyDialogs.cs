using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using Windows.UI.Popups;
using WinRTXamlToolkit.Controls;

namespace IT_Jakub.Classes.Utils {
    static class MyDialogs {


        public static async void showDialogOK(string message) {
            var dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand("OK"));
            await dialog.ShowAsync();
        }

        public static async void showDialogOK(string message, UICommandInvokedHandler action) {
            MessageDialog dialog;
            dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand("OK", action));
            await dialog.ShowAsync();
        }

        internal static async Task<CredentialPickerResults> showLoginDialog(bool isCredentialsCorrect) {
            CredentialPickerOptions credPickerOptions = new CredentialPickerOptions();
            credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Basic;
            credPickerOptions.Message = "Pro používání této aplikace je nutné se přihlásit.\nPřihlaste se pomocí uživatelského jména a hesla níže.";
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
