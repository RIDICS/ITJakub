using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    abstract class ICommand {

        internal const string SYNCHRONIZED_READING_APPLICATION = "501";
        internal Command commandState;

        internal const char SEPARATOR = ':';
        internal const string GENERAL = "000";
        internal const string USER = "User";
        internal string[] commandArray;
        internal string appCommand;
        internal string commandObject;
        internal string command;

        abstract internal Task<bool> procedeCommand();
    }
}
