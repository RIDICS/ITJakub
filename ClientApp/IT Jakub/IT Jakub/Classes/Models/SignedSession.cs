using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    class SignedSession {

        private Session sessionData = null;
        private static bool signedState = false;
        private static LoggedUser lu = LoggedUser.getInstance();
        private long latestCommandId = 0;
        

        public static bool isSignedIn {
            get {
                return signedState;
            }
        }

        private static SignedSession instance = null;

        public static SignedSession getInstance() {
            if (instance == null) {
                instance = new SignedSession();
            }
            return instance;
        }

        private SignedSession() {
            
        }

        private void setSessionData(Session s) {
            sessionData = s;
        }

        internal Session getSessionData() {
            return sessionData;
        }

        public void register(Session s) {
            setSessionData(s);
            signedState = true;
        }

        public async void login() {
            SessionUserTable sut = new SessionUserTable();
            await sut.signOutUserFromAllSessions(lu.getUserData());
            sut.loginUserInSession(lu.getUserData(), this.sessionData);

            CommandTable ct = new CommandTable();
            List<Command> items = await ct.getAllSessionCommands(this.sessionData);
            for (int i = 0; i < items.Count; i++) {
                await items[i].procedeCommand();
            }
            await sendCommand(Command.getUserLoggedInCommand(lu.getUserData()));
        }

        internal void setLatestCommandId(long id) {
            latestCommandId = id;
        }

        internal long getLatestCommandId() {
            return latestCommandId;
        }

        public async Task signout() {
            TaskKiller.killEducationalApplicationTasks();
            SessionUserTable sut = new SessionUserTable();
            CommandTable ct = new CommandTable();
            
            await sut.removeUserFromSession(this.getSessionData(), lu.getUserData());
            await sendCommand(Command.getUserLoggedOutCommand(lu.getUserData()));
            await ct.removeUserLoginLogoutCommands(sessionData, lu.getUserData());
            
            setSessionData(null);
            signedState = false;
        }

        public async Task<bool> sendCommand(string commandText) {
            CommandTable ct = new CommandTable();
            long sentCommandId = await ct.createCommand(sessionData, lu.getUserData(), commandText);
            if (sentCommandId > latestCommandId) {
                latestCommandId = sentCommandId;
                return true;
            }
            return false;
        }

        internal async void promoteUser(long userId) {
            sessionData.PrefferedUserId = userId;
            CommandTable ct = new CommandTable();
            await ct.deletePrevPromoteDemoteCommands(sessionData);
        }

        internal async void demoteUser(long userId) {
            if (sessionData.PrefferedUserId == userId) {
                sessionData.PrefferedUserId = 0;
                CommandTable ct = new CommandTable();
                await ct.deletePrevPromoteDemoteCommands(sessionData);
            }
        }

        internal bool isSignedInSession() {
            return isSignedIn;
        }
    }
}
