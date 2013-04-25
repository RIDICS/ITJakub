using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    class LoggedUser {

        private User userData = null;
        private static bool loggedState = false;
        public static bool isLoogedIn {
            get {
                return loggedState;
            }
        }
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
            loggedState = true;
        }

        public async Task<bool> logout() {
            SessionUserTable sut = new SessionUserTable();
            await sut.signOutUserFromAllSessions(this.userData);
            await ss.signout();
            
            setUserData(null);
            loggedState = false;
            return true;
        }

    }
}
