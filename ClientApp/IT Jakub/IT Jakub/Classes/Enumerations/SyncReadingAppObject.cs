using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Enumerations {
    /// <summary>
    /// Enumeration Class of users rights in the system.
    /// </summary>
    public sealed class SyncReadingAppObject {
        /// <summary>
        /// Command targets at the whole Crosswords Application
        /// </summary>
        public const string WHOLE_APPLICATION = "App";
        /// <summary>
        /// Command tagets at actual text in application.
        /// </summary>
        public const string TEXT = "Text";
        /// <summary>
        /// Command targets at Reading text pointer.
        /// </summary>
        public const string POINTER = "Pointer";
    }
}
