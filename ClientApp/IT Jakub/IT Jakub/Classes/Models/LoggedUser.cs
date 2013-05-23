using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    /// <summary>
    /// Singleton class of currently logged user.
    /// </summary>
    class LoggedUser {

        /// <summary>
        /// The users data
        /// </summary>
        private User userData = null;
        /// <summary>
        /// If is user logged in
        /// </summary>
        private static bool userLoggedIn = false;

        /// <summary>
        /// The instance of singleton class.
        /// </summary>
        private static LoggedUser instance;
        /// <summary>
        /// The ss is singleton instance of SignedSession, Session where is user currently logged in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static LoggedUser getInstance() {
            if (instance == null) {
                instance = new LoggedUser();
            }
            return instance;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="LoggedUser"/> class from being created from outside this class.
        /// </summary>
        private LoggedUser() {
            
        }

        /// <summary>
        /// Sets the user data.
        /// </summary>
        /// <param name="u">The logged User</param>
        private void setUserData(User u) {
            userData = u;
        }

        /// <summary>
        /// Gets the user data.
        /// </summary>
        /// <returns></returns>
        public User getUserData() {
            return userData;
        }

        /// <summary>
        /// Logins the specified User.
        /// </summary>
        /// <param name="u">The u.</param>
        public void login(User u) {
            setUserData(u);
            userLoggedIn = true;
        }

        /// <summary>
        /// Logouts this User and sign him out from all sessions.
        /// </summary>
        /// <returns></returns>
        public async Task logout() {
            if (userLoggedIn == true) {
                userLoggedIn = false;
                SessionUserTable sut = new SessionUserTable();
                if (SignedSession.isSignedIn) {
                    await ss.signout();
                }
                try {
                    await sut.signOutUserFromAllSessions(this.userData);
                } catch (Exception e) {
                    object o = e;
                    return;
                }
                setUserData(null);
            }
        }

        /// <summary>
        /// Determines whether User [is logged in].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is logged in]; otherwise, <c>false</c>.
        /// </returns>
        internal bool isLoggedIn() {
            return userLoggedIn;
        }

    }
}
