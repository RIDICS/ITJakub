using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Enumerations {
    /// <summary>
    /// Enumeration Class of users rights in the system.
    /// </summary>
    public sealed class ApplicationType {
        /// <summary>
        /// The GENERAL command is used for whatever application
        /// </summary>
        public const string GENERAL = "000";
        /// <summary>
        /// The command is used for Syncronized application only.
        /// </summary>
        public const string SYNCHRONIZED_READING_APPLICATION = "501";
        /// <summary>
        /// The command is used for Crossword application only.
        /// </summary>
        public const string CROSSWORDS_APPLICATION = "502";
    }
}
