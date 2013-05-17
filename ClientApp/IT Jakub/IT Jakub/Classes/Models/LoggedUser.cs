using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    class LoggedUser {

        private User userData = null;
        private static bool userLoggedIn = false;

        private static LoggedUser instance;
        private static SignedSession ss = SignedSession.getInstance();

        public static LoggedUser getInstance() {
            if (instance == null) {
                instance = new LoggedUser();
            }
            return instance;
        }

        private LoggedUser() {
            
        }
        
        private void setUserData(User u) {
            userData = u;
        }

        public User getUserData() {
            return userData;
        }

        public void login(User u) {
            setUserData(u);
            userLoggedIn = true;
        }

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
                }
                setUserData(null);
            }
        }

        internal bool isLoggedIn() {
            return userLoggedIn;
        }

    }
}
