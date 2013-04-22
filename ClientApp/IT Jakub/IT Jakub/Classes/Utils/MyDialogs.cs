using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

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
    }
}
