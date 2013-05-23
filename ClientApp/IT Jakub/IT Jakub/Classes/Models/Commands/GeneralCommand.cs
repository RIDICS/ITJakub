using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
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
    /// <summary>
    /// This command represents command which is not specified for particular application, all aplications should have know about this type of command.
    /// </summary>
    class GeneralCommand : Command {

        /// <summary>
        /// The c is representation of the particular Command.
        /// </summary>
        private Command c;
        /// <summary>
        /// The ss is instance of singleton SignedSession class.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();

        /// <summary>
        /// The lu is instance of singleton LoggedUser class.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralCommand"/> class.
        /// </summary>
        /// <param name="c">The Command which creates this instance.</param>
        public GeneralCommand(Command c) {
            this.c = c;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns></returns>
        internal async override Task<bool> procedeCommand() {
            switch (c.commandObject) {
                case GeneralApplicationObject.USER:
                    procedeUserCommand();
                    break;
            }
            return true;
        }

        /// <summary>
        /// Procedes command which targets at User functions.
        /// </summary>
        private void procedeUserCommand() {
            if (c.CommandText.Contains("Login(")) {
                procedeLoginCommand();
            }
            if (c.CommandText.Contains("Logout(")) {
                procedeLogoutCommand();
            }
            if (c.CommandText.Contains("Promote(")) {
                procedePromoteCommand();
            }
            if (c.CommandText.Contains("Demote(")) {
                procedeDemoteCommand();
            }
        }

        /// <summary>
        /// <para>
        /// Procedes the promote command.
        /// </para>
        /// <para>
        /// This method promotes the user in preffered student role.
        /// </para>
        /// </summary>
        private void procedePromoteCommand() {
            string userId = c.command.Replace("Promote(", "");
            userId = userId.Replace(")", "");
            ss.promoteUser(long.Parse(userId));
            Views.EducationalApplications.SynchronizedReading.SyncReadingApp.setUsersRights();
        }

        /// <summary>
        /// <para>
        /// Procedes the demote command.
        /// </para>
        /// <para>
        /// This method demotes the user in his default role.
        /// </para>
        /// </summary>
        private void procedeDemoteCommand() {
            string userId = c.command.Replace("Demote(", "");
            userId = userId.Replace(")", "");
            ss.demoteUser(long.Parse(userId));
            Views.EducationalApplications.SynchronizedReading.SyncReadingApp.setUsersRights();
        }

        /// <summary>
        /// <para>
        /// Procedes the logout command.
        /// </para>
        /// <para>
        /// User has logged out, so applications need to make some operations which handles this situation.
        /// </para>
        /// </summary>
        private void procedeLogoutCommand() {
            Views.EducationalApplications.SynchronizedReading.SyncReadingApp.updateUserList();
        }

        /// <summary>
        /// <para>
        /// Procedes the login command.
        /// </para>
        /// <para>
        /// User has logged in, so applications need to make some operations which handles this situation.
        /// </para>
        /// </summary>
        private void procedeLoginCommand() {
            Views.EducationalApplications.SynchronizedReading.SyncReadingApp.updateUserList();
        }

    }
}
