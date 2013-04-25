using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    class Command : ICommand {

        public long Id { get; set; }
        public long UserId { get; set; }
        public string CommandText { get; set; }
        public long SessionId { get; set; }

        private static SignedSession ss = SignedSession.getInstance();
        
        internal async override Task<bool> procedeCommand() {
            this.commandArray = CommandText.Split(SEPARATOR);
            this.appCommand = commandArray[0];
            this.commandObject = commandArray[1];
            this.command = commandArray[2];

            switch (appCommand) {
                case Command.SYNCHRONIZED_READING_APPLICATION:
                    this.commandState = new SyncronizedReadingApp.SyncReadingAppCommand(this);
                    break;
            }
            await this.commandState.procedeCommand();
            ss.setLatestCommandId(this.Id);
            return true;
        }
    }
}
