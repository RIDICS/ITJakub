using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    abstract class ICommand {

        /// <summary>
        /// The command type which should be executed.
        /// </summary>
        internal ICommand commandState;

        /// <summary>
        /// The array containing splited command.
        /// </summary>
        internal string[] commandArray;
        /// <summary>
        /// The Application type of command.
        /// </summary>
        internal string appCommand;
        /// <summary>
        /// The object in application on whic is command targeted.
        /// </summary>
        internal string commandObject;
        /// <summary>
        /// The actual command.
        /// </summary>
        internal string command;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns></returns>
        abstract internal Task<bool> procedeCommand();
    }
}
