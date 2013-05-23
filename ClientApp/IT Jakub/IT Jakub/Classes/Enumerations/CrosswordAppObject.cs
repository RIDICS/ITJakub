using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Enumerations {
    /// <summary>
    /// Enumeration Class of users rights in the system.
    /// </summary>
    public sealed class CrosswordAppObject {
        /// <summary>
        /// Command targets at the whole Crosswords Application
        /// </summary>
        public const string WHOLE_APPLICATION = "App";
        /// <summary>
        /// Command targets at the particular Crossword in Crosswords Application.
        /// </summary>
        public const string CROSSWORD = "Crossword";
    }
}
