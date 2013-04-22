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

        public async void signout() {
            SessionUserTable sut = new SessionUserTable();
            await sut.removeUserFromSession(this.getSessionData(), lu.getUserData());
            setSessionData(null);
            signedState = false;
        }

        public async Task<bool> sendCommand(string commandText) {
            CommandTable ct = new CommandTable();
            var x = lu;
            bool test = await ct.createCommand(sessionData, lu.getUserData(), commandText);
            return true;
        }
    }
}
