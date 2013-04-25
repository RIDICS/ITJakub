using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace IT_Jakub.Classes.Models.Commands {
    class GeneralCommand : Command {

        RichEditBox textBox = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();

        private Command c;
        private static SignedSession ss = SignedSession.getInstance();
        private static LoggedUser lu = LoggedUser.getInstance();

        public GeneralCommand(Command c) {
            this.c = c;
        }

        internal async override Task<bool> procedeCommand() {
            switch (c.commandObject) {
                case USER:
                    procedeUserCommand();
                    break;
            }
            return true;
        }

        private void procedeUserCommand() {
            if (c.CommandText.Contains("Login(")) {
                procedeLoginCommand();
            }
            if (c.CommandText.Contains("Logout(")) {
                procedeLogoutCommand();
            }
        }

        private async void procedeLogoutCommand() {
            
            
            
            Views.EducationalApplications.SynchronizedReading.SyncReadingApp.updateUserList();
        }

        private void procedeLoginCommand() {
            Views.EducationalApplications.SynchronizedReading.SyncReadingApp.updateUserList();
        }
    }
}
