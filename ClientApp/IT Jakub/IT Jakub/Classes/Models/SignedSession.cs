using IT_Jakub.Classes.DatabaseModels;
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

            await sendCommand(Command.getUserLoggedInCommand(lu.getUserData()));

            CommandTable ct = new CommandTable();
            List<Command> cl = await ct.getAllSessionCommands(this.sessionData);
            LinkedList<Command> cll = new LinkedList<Command>();
            for (int i = 0; i < cl.Count; i++) {
                cll.AddLast(cl[i]);
            }
            for (LinkedListNode<Command> node = cll.First; node != cll.Last.Next; node = node.Next) {
                Command command = node.Value as Command;
                await command.procedeCommand();
            }
        }

        internal void setLatestCommandId(long id) {
            latestCommandId = id;
        }

        internal long getLatestCommandId() {
            return latestCommandId;
        }

        public async Task signout() {
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

        internal async void sendPointerMoveCommand(int pointerIndex) {
            CommandTable ct = new CommandTable();
            string commandText = Classes.Models.Commands.SyncReadingAppCommand.getPointerMoveCommand(pointerIndex);
            await this.sendCommand(commandText);
            removePrevPointerMoveCommands();
        }

        internal void removePrevPointerMoveCommands() {
            CommandTable ct = new CommandTable();
            ct.deletePrevMoveCommands(latestCommandId, sessionData);
        }
    }
}
