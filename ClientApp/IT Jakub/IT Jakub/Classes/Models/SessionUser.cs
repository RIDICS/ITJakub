using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    /// <summary>
    /// Representation of row in database table SessionUser.
    /// </summary>
    class SessionUser {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        public long SessionId { get; set; }
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public long UserId { get; set; }

    }
}
