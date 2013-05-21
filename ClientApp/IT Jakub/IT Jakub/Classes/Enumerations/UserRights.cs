using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Enumerations {
    /// <summary>
    /// Enumeration Class of users rights in the system.
    /// </summary>
    public enum UserRights {
        /// <summary>
        /// The default
        /// </summary>
        Default = 0,
        /// <summary>
        /// The prefered student
        /// </summary>
        Prefered = 1,
        /// <summary>
        /// The owner of session
        /// </summary>
        Owner = 2
    }
}
