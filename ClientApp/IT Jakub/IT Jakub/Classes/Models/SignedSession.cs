using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Networking;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.Controls.FlyoutControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace IT_Jakub.Classes.Models {
    /// <summary>
    /// Singleton class that represents Session in which is user currently signed in.
    /// </summary>
    class SignedSession {

        /// <summary>
        /// The session data
        /// </summary>
        private Session sessionData = null;
        /// <summary>
        /// The signed state, if is user signed in this session.
        /// </summary>
        private static bool signedState = false;
        /// <summary>
        /// The lu is singleton instance of currently logged user.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();
        /// <summary>
        /// The latest command id, for geting just new commands from database.
        /// </summary>
        private long latestCommandId = 0;


        /// <summary>
        /// Gets a value indicating whether user is signed in the session.
        /// </summary>
        /// <value>
        /// <c>true</c> if this user is signed in; otherwise, <c>false</c>.
        /// </value>
        public static bool isSignedIn {
            get {
                return signedState;
            }
        }

        /// <summary>
        /// The instance of this class.
        /// </summary>
        private static SignedSession instance = null;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static SignedSession getInstance() {
            if (instance == null) {
                instance = new SignedSession();
            }
            return instance;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SignedSession"/> class from being created from outside of this class.
        /// </summary>
        private SignedSession() {
            
        }

        /// <summary>
        /// Sets the session data.
        /// </summary>
        /// <param name="s">The session</param>
        private void setSessionData(Session s) {
            sessionData = s;
        }

        /// <summary>
        /// Gets the session data.
        /// </summary>
        /// <returns></returns>
        internal Session getSessionData() {
            return sessionData;
        }

        /// <summary>
        /// Registers the user in specified session.
        /// </summary>
        /// <param name="s">The session where user need to be registered</param>
        public void register(Session s) {
            setSessionData(s);
            signedState = true;
        }

        /// <summary>
        /// Logins user in registered session.
        /// </summary>
        public async void login() {
            SessionUserTable sut = new SessionUserTable();
            await sut.signOutUserFromAllSessions(lu.getUserData());
            sut.loginUserInSession(lu.getUserData(), this.sessionData);

            CommandTable ct = new CommandTable();
            try {
                List<Command> items = await ct.getAllSessionCommands(this.sessionData);
                for (int i = 0; i < items.Count; i++) {
                    await items[i].procedeCommand();
                }
                await sendCommand(CommandBuilder.getUserLoggedInCommand(lu.getUserData()));
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
            }
        }

        /// <summary>
        /// Sets the latest command id.
        /// </summary>
        /// <param name="id">The command id.</param>
        internal void setLatestCommandId(long id) {
            if (latestCommandId < id) {
                latestCommandId = id;
            }
        }

        /// <summary>
        /// Gets the latest command id.
        /// </summary>
        /// <returns></returns>
        internal long getLatestCommandId() {
            return latestCommandId;
        }

        /// <summary>
        /// Signouts user from the session.
        /// </summary>
        /// <returns></returns>
        public async Task signout() {
            TaskKiller.killEducationalApplicationTasks();
            SessionUserTable sut = new SessionUserTable();
            CommandTable ct = new CommandTable();
            
            await sut.removeUserFromSession(this.getSessionData(), lu.getUserData());
            await sendCommand(CommandBuilder.getUserLoggedOutCommand(lu.getUserData()));
            await ct.removeUserLoginLogoutCommands(sessionData, lu.getUserData());
            
            setSessionData(null);
            signedState = false;
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="commandText">The text of command.</param>
        /// <returns></returns>
        public async Task<long> sendCommand(string commandText) {
            CommandTable ct = new CommandTable();
            try {
                long sentCommandId = await ct.createCommand(sessionData, lu.getUserData(), commandText);
                if (sentCommandId > latestCommandId) {
                    latestCommandId = sentCommandId;
                }
                return sentCommandId;
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return -1;
            }
        }

        /// <summary>
        /// Promotes the user in prefferedStudent Role.
        /// </summary>
        /// <param name="userId">The user id.</param>
        internal async void promoteUser(long userId) {
            sessionData.PrefferedUserId = userId;
            CommandTable ct = new CommandTable();
            await ct.deletePrevPromoteDemoteCommands(sessionData);
        }

        /// <summary>
        /// Demotes the user in his default Role.
        /// </summary>
        /// <param name="userId">The user id.</param>
        internal async void demoteUser(long userId) {
            if (sessionData.PrefferedUserId == userId) {
                sessionData.PrefferedUserId = 0;
                CommandTable ct = new CommandTable();
                await ct.deletePrevPromoteDemoteCommands(sessionData);
            }
        }

        /// <summary>
        /// Determines whether user [is signed in session].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if user [is signed in session]; otherwise, <c>false</c>.
        /// </returns>
        internal bool isSignedInSession() {
            return isSignedIn;
        }
    }
}
