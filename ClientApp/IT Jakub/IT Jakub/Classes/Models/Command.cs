using IT_Jakub.Classes.Enumerations;
using IT_Jakub.Classes.Models.Commands;
using IT_Jakub.Classes.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    /// <summary>
    /// <para>
    /// Common command, this class determins what type of command should be used.
    /// </para>
    /// <para>
    /// Also this class is representation of row in database table Command.
    /// </para>
    /// </summary>
    class Command : ICommand {

        /// <summary>
        /// Gets or sets the Command.Id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the Command.UserId this user has created this command.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public long UserId { get; set; }
        /// <summary>
        /// Gets or sets the Command.CommandText which is actual text of the command.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        public string CommandText { get; set; }
        /// <summary>
        /// Gets or sets the Command.SessionId. Session for which is command determined.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        public long SessionId { get; set; }

        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();

        /// <summary>
        /// Executes the command and determins what type of command may be used.
        /// </summary>
        /// <returns></returns>
        internal async override Task<bool> procedeCommand() {
            this.commandArray = CommandText.Split(CommandBuilder.SEPARATOR);
            this.appCommand = commandArray[0];
            this.commandObject = commandArray[1];
            this.command = commandArray[2];

            switch (appCommand) {
                case ApplicationType.SYNCHRONIZED_READING_APPLICATION:
                    this.commandState = new SyncReadingAppCommand(this);
                    break;
                case ApplicationType.CROSSWORDS_APPLICATION:
                    this.commandState = new CrosswordsAppCommand(this);
                    break;
                case ApplicationType.GENERAL:
                    this.commandState = new GeneralCommand(this);
                    break;
            }
            await this.commandState.procedeCommand();
            ss.setLatestCommandId(this.Id);
            return true;
        }
    }
}
